using MyBankApp.Application.Configuration;
using MyBankApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Persistence.Helper
{
    public class TokenConfirmation
    {
        private readonly IUnitOfWork _unitOfWork;
        public TokenConfirmation(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> ConfirmToken(string token, string email)
        {
            var verificationToken = await _unitOfWork.VerificationTokens.GetByColumnAsync(x => x.Email == email && x.Token == token && x.ActionType == ActionType.EmailConfirmation.ToString());

            if (verificationToken != null && verificationToken.DateCreated.AddMinutes(10) > DateTime.UtcNow)
            {
                await _unitOfWork.VerificationTokens.DeleteAsync(verificationToken);
                await _unitOfWork.CompleteAsync();
                return true;
            }

            return false;
        }
    }
}
