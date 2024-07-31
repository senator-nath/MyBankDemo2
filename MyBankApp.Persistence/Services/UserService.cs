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

        private readonly RandomNumberGenerator _randomNumberGenerator;
        private readonly AgeCalculator _ageCalculator;
        private readonly TokenGenerator _tokenGenerator;
        private readonly TokenConfirmation _tokenConfirmation;

        private readonly ConfirmationMail _confirmMail;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IConfiguration configuration, UserRequestValidator validator, IOptions<AppSettings> appSettings, IHttpClientFactory httpClientFactory, RandomNumberGenerator randomNumberGenerator, AgeCalculator ageCalculator, TokenGenerator tokenGenerator, TokenConfirmation tokenConfirmation, ConfirmationMail confirmMail)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
            _appSettings = appSettings.Value;
            _ageCalculator = ageCalculator;
            _tokenGenerator = tokenGenerator;
            _randomNumberGenerator = randomNumberGenerator;
            _tokenConfirmation = tokenConfirmation;
            _confirmMail = confirmMail;
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
                    Token = await _tokenGenerator.GenerateToken(user.Email, "EmailConfirmation"),
                    FirstName = user.FirstName
                };

                await _unitOfWork.user.CreateAsync(user);
                await _confirmMail.SendConfirmationEmail(emailObject);
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


        public async Task<string> EmailConfirmation(EmailConfirmationRequestDto request)
        {
            var user = await _unitOfWork.user.GetByColumnAsync(x => x.Email == request.UserEmail);

            if (await _tokenConfirmation.ConfirmToken(request.Token, request.UserEmail))
            {
                user.EmailConfirmed = true;
                await _unitOfWork.user.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();
                return "Your Email has beem confirmed";
            }
            return "Email Confirmation Failed";
        }



        public async Task<UserResponseDetails> ResetPasswordRequest(ResetPasswordRequestDto entity)
        {
            var response = new UserResponseDetails();
            var user = await _unitOfWork.user.GetByColumnAsync(x => x.Email == entity.Email);

            if (user == null)
            {
                response.Message = "User does not exist";
                return response;
            }

            var token = await _tokenGenerator.GenerateToken(user.Email, "EmailConfirmation");
            var emailObject = new EmailConfirmationRequestDto
            {
                UserEmail = user.Email,
                Token = token,
                FirstName = user.FirstName
            };

            await _confirmMail.SendResetPasswordEmail(emailObject);
            response.Message = "Reset password token sent to your email. Please check your email to complete the reset process.";
            response.IsSuccess = true;

            return response;
        }


        public async Task<string> ResetPasswordWithToken(ResetPasswordRequestTokenDto request)
        {
            var user = await _unitOfWork.user.GetByColumnAsync(x => x.Email == request.Email);

            if (user == null)
            {
                return "User not found";
            }

            if (user.Status != "isInactive")
            {
                return "Account is not locked";
            }

            var verificationToken = await _unitOfWork.VerificationTokens.GetByColumnAsync(x => x.Email == request.Email && x.ActionType == ActionType.ResetPassword.ToString());

            if (!await _tokenConfirmation.ConfirmToken(request.Token, request.Email))
            {
                return "Invalid or expired token";
            }

            user.HashPassword = Helper.Helper.HashPassword(request.NewPassword);
            user.Status = "isActive";
            user.LoginAttempts = 0;
            await _unitOfWork.user.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            return "Password reset successful";
        }



    }
}



