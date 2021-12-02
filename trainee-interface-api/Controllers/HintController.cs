using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using trainee_interface_api.Contexts;
using trainee_interface_api.Models.DTO;
using trainee_interface_api.Models;
using System;

namespace trainee_interface_api.Controllers
{
    [Route("[controller]")]
    [ApiController]


    public class HintController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public HintController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("gethint/{hintId}")]
        public async Task<IActionResult> GetHintByHintId(int hintId)
        {
            if (hintId == 0)
            {
                return BadRequest(new ApiResponse<string>(false, "A hint with this Id does not exist"));
            }
            var hint = await _dbContext.Hints.Where(x => x.HintId == hintId).FirstOrDefaultAsync();
            DisplayHint displayHint = new DisplayHint(hint.HintText, hint.ImageUrl);
            return Ok(new ApiResponse<DisplayHint>(true, displayHint));
        }

        [HttpGet("gethintscenario/{scenarioId}/{hintId}")]
        public async Task<IActionResult> GetHintByScenarioAndHintId(int scenarioId, int hintId)
        {
            if (hintId == 0)
            {
                return BadRequest(new ApiResponse<string>(false, "A hint with this Id does not exist"));
            }
            var hint = await _dbContext.Hints.Where(x => x.HintId == hintId && x.ScenarioId == scenarioId).FirstOrDefaultAsync();
            DisplayHint displayHint = new DisplayHint(hint.HintText, hint.ImageUrl);
            return Ok(new ApiResponse<DisplayHint>(true, displayHint));
        }

        [HttpGet("getnexthint/{teamId}")]
        public async Task<IActionResult> GetNextHint(int teamId)
        {
            if (teamId == 0)
            {
                return BadRequest(new ApiResponse<string>(false, "no team found"));
            }
            var currentScenario = await _dbContext.StartedScenarios.Include(x => x.Scenario).FirstOrDefaultAsync(x => x.Team.Id == teamId && x.EndTime == null);
            var teamHint = await _dbContext.TeamHints.Where(x => x.TeamId == currentScenario.Id).FirstOrDefaultAsync();
            var team = await _dbContext.Teams.Where(x => x.Id == teamId).FirstOrDefaultAsync();
            var hint = await _dbContext.Hints.Where(x => x.HintId == teamHint.HintId).FirstOrDefaultAsync();
            if (hint != null)
            {
                teamHint.HintId = teamHint.HintId + 1;
                await _dbContext.SaveChangesAsync();
            }
            HintLog hintLog = new HintLog(team.Name, hint.HintId, hint.ScenarioId);
            await _dbContext.HintLogs.AddAsync(hintLog);
            await _dbContext.SaveChangesAsync();

            if (hint == null)
            {
                return BadRequest(new ApiResponse<string>(false, "There is no remaining hints for this scenario left"));
            }

            DisplayHint displayHint = new DisplayHint(hint.HintText, hint.ImageUrl);
            return Ok(new ApiResponse<DisplayHint>(true, displayHint));

        }
    }
}
