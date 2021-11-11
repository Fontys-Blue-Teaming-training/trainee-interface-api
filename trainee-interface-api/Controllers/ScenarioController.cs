﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using trainee_interface_api.Contexts;
using trainee_interface_api.Models;
using trainee_interface_api.Models.DTO;

namespace trainee_interface_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScenarioController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public ScenarioController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetScenarios()
        {

            return Ok(new ApiResponse<List<Scenario>>(true, await _dbContext.Scenarios.ToListAsync()));
        }

        //[HttpPost]
        //public async Task<IActionResult> StartScenario(ToggleScenario toggleScenario)
        //{
        //    if(toggleScenario == default)
        //    {
        //        return BadRequest(new ApiResponse<string>(false, "ToggleScenario cannot be empty"));
        //    }

        //    var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == toggleScenario.TeamId);
        //    if (team == default)
        //    {
        //        return BadRequest(new ApiResponse<string>(false, "TeamId does not exist"));
        //    }

        //    var scenario = await _dbContext.Scenarios.FirstOrDefaultAsync(x => x.Id == toggleScenario.ScenarioId);
        //    if (scenario == default)
        //    {
        //        return BadRequest(new ApiResponse<string>(false, "ScenarioId does not exist"));
        //    }
        //}
    }
}
