using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace MyBankApp.Persistence.Helper
{

    public class TokenGenerator
    {
        private readonly AppSettings _appSettings;

        public TokenGenerator(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GenerateToken(string userName)
        {
            var claim = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
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
            return tokenHandler.WriteToken(tok);
        }
    }
}

