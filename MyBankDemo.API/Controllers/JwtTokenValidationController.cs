using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBankApp.Application.Contracts.IServices;
using MyBankApp.Persistence.Services;
using System.Threading.Tasks;

namespace MyBankDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtTokenValidationController : ControllerBase
    {
        private readonly IJwtValidationService _tokenValidationService;

        public JwtTokenValidationController(IJwtValidationService tokenValidationService)
        {
            _tokenValidationService = tokenValidationService;
        }

        //[HttpPost("validate")]
        //public async Task<bool> ValidateToken([FromBody] string token)
        //{
        //    return await _tokenValidationService.ValidateTokenAsync(token);
        //}
        [HttpPost]
        public async Task<IActionResult> ValidateJwtToken(string token)
        {
            var isValid = await _tokenValidationService.ValidateJwtToken(token);
            if (isValid)
            {
                return Ok();
            }

            return Unauthorized();
        }
    }
}
