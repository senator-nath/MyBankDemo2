using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBankApp.Application.Configuration;
using MyBankApp.Application.Contracts.IServices;
using MyBankApp.Application.validator;
using MyBankApp.Domain.Dto.RequestDto;
using MyBankApp.Domain.Dto.ResponseDto;
using MyBankApp.Domain.Entities;
using MyBankApp.Persistence.Services;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MyBankDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDetails>> RegisterUser([FromBody] UserRequestDto user)
        {
            try
            {
                var result = await _userService.Register(user);
                return result;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDetails>> Login(LoginRequestDto entity)
        {
            return await _userService.Login(entity);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePassWordRequestDto entity)
        {
            var response = await _userService.ChangePassword(entity);
            return Ok(response);
        }

        [HttpPost]
        [Route(" EmailConfirmation")]
        public async Task<string> EmailConfirmation(EmailConfirmationRequestDto request)
        {
            var result = await _userService.EmailConfirmation(request);
            return result;
        }
        [HttpPost("request")]
        public async Task<ActionResult<UserResponseDetails>> ResetPasswordRequest(ResetPasswordRequestDto entity)
        {
            var response = await _userService.ResetPasswordRequest(entity);
            return response;
        }
        [HttpPost("with-token")]
        public async Task<ActionResult<string>> ResetPasswordWithToken(ResetPasswordRequestTokenDto entity)
        {
            var response = await _userService.ResetPasswordWithToken(entity);
            return response;
        }
    }
}
