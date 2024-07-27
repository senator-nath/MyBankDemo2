using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyBankApp.Application.Configuration;
using MyBankApp.Application.Contracts.IServices;
using MyBankApp.Application.validator;
using MyBankApp.Domain.Dto.RequestDto;
using MyBankApp.Domain.Dto.ResponseDto;
using MyBankApp.Domain.Entities;
using MyBankApp.Domain.Enum;
using MyBankApp.Persistence.Helper;
using MyBankApp.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MyBankApp.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;
        private readonly UserRequestValidator _validator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RandomNumberGenerator _randomNumberGenerator;
        private readonly AgeCalculator _ageCalculator;
        private readonly TokenGenerator _tokenGenerator;
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IConfiguration configuration, UserRequestValidator validator, IOptions<AppSettings> appSettings, IHttpClientFactory httpClientFactory, RandomNumberGenerator randomNumberGenerator, AgeCalculator ageCalculator, TokenGenerator tokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
            _appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
            _ageCalculator = ageCalculator;
            _tokenGenerator = tokenGenerator;
            _randomNumberGenerator = randomNumberGenerator;
        }
        public async Task<UserResponseDetails> Login(LoginRequestDto entity)
        {
            var response = new UserResponseDetails();
            var user = await _unitOfWork.user.GetByColumnAsync(x => x.Email == entity.Email);

            if (user == null || !user.EmailConfirmed)
            {
                response.Message = user == null ? "User does not exist" : "Please confirm your email before logging in";
                return response;
            }
            if (user.Status == "isInactive")
            {
                response.Message = "Account is locked. Please reset password to access your account.";
                return response;
            }

            var password = Helper.Helper.HashPassword(entity.Password);
            if (!password.Equals(user.HashPassword))
            {
                user.LoginAttempts++;
                await _unitOfWork.user.UpdateAsync(user);

                if (user.LoginAttempts >= 3)
                {
                    user.Status = "isInactive";
                    await _unitOfWork.user.UpdateAsync(user);
                    response.Message = "Account locked due to multiple failed login attempts. Please reset password to access your account.";
                }
                else
                {
                    response.Message = "Email or password is incorrect";
                }
                return response;
            }

            // Login successful, reset login attempts and unlock account if necessary
            user.LoginAttempts = 0;
            user.Status = "isActive";
            await _unitOfWork.user.UpdateAsync(user);

            var token = _tokenGenerator.GenerateToken(user.UserName);

            response.Token = token;
            response.Message = "Login successful";
            response.IsSuccess = true;

            return response;
        }
        public async Task<UserResponseDetails> Register(UserRequestDto entity)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(entity);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                var userExist = await _unitOfWork.user.GetByColumnAsync(x => x.Email == entity.Email);
                if (userExist != null)
                {
                    return new UserResponseDetails()
                    {
                        Message = $"User with the email {entity.Email} already exists. Please login",
                        IsSuccess = false
                    };
                }

                var user = new User
                {
                    Email = entity.Email,
                    FirstName = entity.FirstName,
                    MiddleName = entity.MiddleName,
                    Address = entity.Address,
                    LastName = entity.LastName,
                    PhoneNumber = entity.PhoneNumber,
                    Dob = entity.Dob,
                    Gender = entity.Gender,
                    StateId = entity.StateId,
                    LGAId = entity.LGAId,
                    Age = _ageCalculator.CalculateAgeFromDateOfBirth(entity.Dob),
                    UserName = entity.UserName,
                    Title = entity.Title,
                    accountType = entity.AccountType,
                    LandMark = entity.LandMark,
                    NIN = entity.NIN,
                    HasBvn = entity.HasBvn,
                    Bvn = entity.Bvn,
                    HashPassword = Helper.Helper.HashPassword(entity.Password),
                    AccountNo = _randomNumberGenerator.Generate11DigitRandomNumber(),
                    Status = "isActive"
                };

                var emailObject = new EmailConfirmationRequestDto
                {
                    UserEmail = user.Email,
                    Token = _tokenGenerator.GenerateToken(user.UserName),
                    FirstName = user.FirstName
                };

                await _unitOfWork.user.CreateAsync(user);
                await SendConfirmationEmail(emailObject);
                await _unitOfWork.CompleteAsync();

                return new UserResponseDetails()
                {
                    IsSuccess = true,
                    Message = "Registration successful. Please confirm your email to login."
                };
            }
            catch (ValidationException ex)
            {
                return new UserResponseDetails()
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                return new UserResponseDetails()
                {
                    Message = "Server error",
                    IsSuccess = false
                };
            }
        }
        public async Task<UserResponseDetails> ChangePassword(ChangePassWordRequestDto entity)
        {
            try
            {
                var user = await _unitOfWork.user.GetByColumnAsync(x => x.Email == entity.Email);

                if (user == null)
                {
                    return new UserResponseDetails()
                    {
                        Message = "User not found",
                        IsSuccess = false
                    };
                }

                var password = Helper.Helper.HashPassword(entity.OldPassword);
                if (!password.Equals(user.HashPassword))
                {
                    return new UserResponseDetails()
                    {
                        Message = "Old password is incorrect",
                        IsSuccess = false
                    };
                }

                if (entity.NewPassword != entity.ConfirmPassword)
                {
                    return new UserResponseDetails()
                    {
                        Message = "New password and confirm password do not match",
                        IsSuccess = false
                    };
                }

                user.HashPassword = Helper.Helper.HashPassword(entity.NewPassword);
                await _unitOfWork.user.UpdateAsync(user);

                return new UserResponseDetails()
                {
                    Message = "Password changed successfully",
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return new UserResponseDetails()
                {
                    Message = "Server error",
                    IsSuccess = false
                };
            }
        }


        public async Task<UserResponseDetails> ResetPassword(ResetPasswordRequestDto entity)
        {
            var response = new UserResponseDetails();
            var user = await _unitOfWork.user.GetByColumnAsync(x => x.Email == entity.Email);

            if (user == null)
            {
                response.Message = "User does not exist";
                return response;
            }

            if (user.Status != "isInactive")
            {
                response.Message = "Account is already active. No need to reset password.";
                return response;
            }

            user.HashPassword = Helper.Helper.HashPassword(entity.NewPassword);
            user.Status = "isActive";
            user.LoginAttempts = 0;
            await _unitOfWork.user.UpdateAsync(user);

            response.Message = "Password reset successful. You can now login.";
            response.IsSuccess = true;

            return response;
        }

        public async Task SendConfirmationEmail(EmailConfirmationRequestDto request)
        {
            var httpclient = _httpClientFactory.CreateClient();
            var emailModel = new
            {
                To = request.UserEmail,
                Subject = "Account Registration",
                Body = $"Hello {request.FirstName}, here is ur verification token: {request.Token}"

            };
            var sendEmail = await httpclient.PostAsJsonAsync("https://localhost:7168/api/EmailService", emailModel);


        }
        public async Task<string> EmailConfirmation(EmailConfirmationRequestDto request)
        {
            var user = await _unitOfWork.user.GetByColumnAsync(x => x.Email == request.UserEmail);

            if (await ConfirmToken(request.Token, request.UserEmail))
            {
                user.EmailConfirmed = true;
                await _unitOfWork.user.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();
                return "Your Email has beem confirmed";
            }
            return "Email Confirmation Failed";
        }

        public async Task<bool> ConfirmToken(string token, string email)
        {
            var verificationToken = new VerificationToken
            {
                Email = email,
                Token = token,
                ActionType = ActionType.EmailConfirmation.ToString(),

            };
            await _unitOfWork.VerificationTokens.CreateAsync(verificationToken);
            await _unitOfWork.CompleteAsync();
            var userWithToken = await _unitOfWork.VerificationTokens.GetByColumnAsync(x => x.Email == email && x.Token == token);

            if (userWithToken.Email == email && userWithToken.Token == token)
            {
                return true;
            }
            return false;
        }
    }


}


