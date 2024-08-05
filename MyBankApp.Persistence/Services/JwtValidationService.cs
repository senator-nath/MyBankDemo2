using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyBankApp.Application.Contracts.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Persistence.Services
{
    public class JwtValidationService : IJwtValidationService
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;

        public JwtValidationService(IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _configuration = configuration;
            _appSettings = appSettings.Value;
        }


        public async Task<bool> ValidateJwtToken(string token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:secret"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Adjust if needed
                }, out SecurityToken validatedToken);

                // Optionally, you can extract claims and other information from the validatedToken here
                var jwtToken = (JwtSecurityToken)validatedToken;
                // var userId = jwtToken.Claims.First(x => x.Type == "id").Value;
                var userNameClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                var userName = userNameClaim != null ? userNameClaim.Value : "No Name claim";

                // Return true if validation passes
                return true;
            }
            catch
            {
                // Return false if validation fails
                return false;
            }
        }
    }
}
