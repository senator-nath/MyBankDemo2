using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBankApp.Application.Contracts.IServices;
using MyBankApp.Persistence.Services;
using System.Threading.Tasks;

namespace MyBankDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LGAController : ControllerBase
    {
        private readonly ILGAService _lgaService;

        public LGAController(ILGAService lgaService)
        {
            _lgaService = lgaService;
        }

        [HttpGet("GetLGAsByStateId/{stateId:int}")]
        public async Task<IActionResult> GetLGAsByStateId(int stateId)
        {
            var lgas = await _lgaService.GetLGAsByStateIdAsync(stateId);
            return Ok(lgas);
        }
    }
}
