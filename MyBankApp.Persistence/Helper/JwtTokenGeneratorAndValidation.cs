using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace MyBankApp.Persistence.Helper
{
    public class JwtTokenGeneratorAndValidation
    {
        private readonly AppSettings _appSettings;

        public JwtTokenGeneratorAndValidation(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GenerateToken(string userName)
        {
            // Define token claims
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var expiration = DateTime.UtcNow.AddDays(1);


            var tokenDescriptor = new JwtSecurityToken(
                expires: expiration,
                signingCredentials: credentials,
                claims: claims
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenDescriptor);
        }


    }
}
