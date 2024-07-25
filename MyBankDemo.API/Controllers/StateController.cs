using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBankApp.Application.Contracts.IServices;
using MyBankApp.Domain.Entities;
using MyBankApp.Persistence.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBankDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<State>>> GetAllStates()
        {
            var states = await _stateService.GetAllStatesAsync();
            return Ok(states);
        }
    }
}
