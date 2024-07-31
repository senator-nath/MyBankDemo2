using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using MyBankApp.Domain.Entities;
using MyBankApp.Domain.Enum;
using MyBankApp.Application.Configuration;

namespace MyBankApp.Persistence.Helper
{

    public class TokenGenerator
    {
        private readonly IUnitOfWork _unitOfWork;
        public TokenGenerator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateToken(string email, string actionType)
        {
            var token = new Random().Next(10000, 99999);
            var expiresAt = DateTime.UtcNow.AddMinutes(2);

            var verificationToken = new VerificationToken
            {
                Email = email,
                Token = token.ToString(),
                ActionType = actionType,
                DateCreated = DateTime.UtcNow
            };

            await _unitOfWork.VerificationTokens.CreateAsync(verificationToken);
            await _unitOfWork.CompleteAsync();

            return token.ToString();
        }
    }
}

