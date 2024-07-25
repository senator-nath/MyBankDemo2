using FluentValidation;
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
using MyBankApp.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IConfiguration configuration, UserRequestValidator validator, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
            _appSettings = appSettings.Value;
        }
        public async Task<UserResponseDetails> Login(LoginRequestDto entity)
        {
            var response = new UserResponseDetails();

            var user = await _unitOfWork.user.GetByColumnAsync(x => x.Email == entity.Email);
            var Password = Helper.Helper.HashPassword(entity.Password);
            if (user != null)
            {
                response.Message = "User does not exist";
            }
            if (!user.Email.Equals(entity.Email))
            {
                response.Message = "Email or password is incorrect";
            }
            if (user.HashPassword != Password)
            {
                response.Message = "Email or password is incorrect";
                return response;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),

                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.Token = tokenHandler.WriteToken(token);
            response.Message = "Login successful";

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

                var user_exist = await _unitOfWork.user.GetByColumnAsync(x => x.Email == entity.Email);

                if (user_exist != null)
                {
                    _logger.LogError("User already exists");
                    return new UserResponseDetails()
                    {
                        Message = $"User with the email {entity.Email} already exists. Please login",
                        IsSuccess = false
                    };
                }

                var user = new User()
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
                    Age = CalculateAgeFromDateOfBirth(entity.Dob),
                    UserName = entity.UserName,
                    Title = entity.Title,
                    accountType = entity.AccountType,
                    LandMark = entity.LandMark,
                    NIN = entity.NIN,
                    HasBvn = entity.HasBvn,
                    Bvn = entity.Bvn,
                    HashPassword = Helper.Helper.HashPassword(entity.Password),
                    AccountNo = Generate11DigitRandomNumber(),
                };

                await _unitOfWork.user.CreateAsync(user);
                await _unitOfWork.CompleteAsync();
                var claim = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, entity.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.secret));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenHandler = new JwtSecurityTokenHandler();
                var exp = DateTime.UtcNow.AddDays(7);
                var tok = new JwtSecurityToken
                    (
                        //issuer: "LocalHost",
                        expires: exp,
                        signingCredentials: cred


                    );
                var tk = new JwtSecurityTokenHandler().WriteToken(tok);

                var response = new UserResponseDto()
                {
                    LastLogin = "Now",
                    Token = tk,
                    DailyLimitBalance = "",
                    AccountNumber = user.AccountNo,
                    UserName = user.UserName,
                    AccountName = user.FirstName + " " + user.MiddleName + " " + user.LastName,
                    Title = user.Title,
                    GenderId = user.GenderId,
                    AccountType = user.accountType,
                    Bvn = user.Bvn,
                    NIN = user.NIN,
                    Status = "",
                };

                return new UserResponseDetails()
                {
                    Message = "",
                    IsSuccess = true,
                    ResponseDetails = response,


                };

            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation error");
                return new UserResponseDetails()
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return new UserResponseDetails()
                {
                    Message = "Server error",
                    IsSuccess = false
                };
            }
        }

        //private string GenerateToken(UserResponseDetails user)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_appSettings.secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //new Claim(ClaimTypes.Name, user.UserName),
        //new Claim(ClaimTypes.Email, user.Email),

        //        }),
        //        Expires = DateTime.UtcNow.AddMinutes(30),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    //    return tokenHandler.WriteToken(token);
        //}
        private static string Generate11DigitRandomNumber()
        {
            Random random = new Random();
            string result = string.Empty;

            result += random.Next(100000, 1000000).ToString("D6");
            result += random.Next(10000, 100000).ToString("D5");

            return result;
        }
        private string CalculateAgeFromDateOfBirth(DateTime Dob)
        {
            var today = DateTime.Today;
            var age = today.Year - Dob.Year;

            if (Dob.Date > today.AddYears(-age)) age--;


            var Age = age.ToString();
            return Age;
        }
    }
}

