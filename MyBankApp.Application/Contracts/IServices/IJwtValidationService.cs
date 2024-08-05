using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Application.Contracts.IServices
{
    public interface IJwtValidationService
    {
        Task<bool> ValidateJwtToken(string token);
    }
}
