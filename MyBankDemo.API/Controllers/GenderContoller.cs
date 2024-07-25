using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBankApp.Application.Configuration;
using MyBankApp.Application.Contracts.IServices;
using MyBankApp.Application.Contracts.Persistence;
using MyBankApp.Domain.Dto.RequestDto;
using MyBankApp.Domain.Dto.ResponseDto;
using MyBankApp.Persistence.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBankDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderContoller : ControllerBase
    {
        private readonly IGenderService _service;

        public GenderContoller(IGenderService service)
        {
            _service = service;

        }
        [HttpGet("Get-ALl-Genders")]
        public async Task<ActionResult<IEnumerable<GenderResponseDto>>> GetAllGenders()
        {
            var genders = await _service.GetAllPostsAsync();


            return Ok(genders);
        }
    }
}
