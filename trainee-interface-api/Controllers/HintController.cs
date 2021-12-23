using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using trainee_interface_api.Contexts;
using trainee_interface_api.Models.DTO;
using trainee_interface_api.Models;
using System;
using System.Collections.Generic;

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
            if (hintId < 1)
            {
                return BadRequest(new ApiResponse<string>(false, "A hint with this Id does not exist"));
            }
            var hint = await _dbContext.Hints.Where(x => x.HintId == hintId).FirstOrDefaultAsync();

            if(hint == default)
            {
                return BadRequest(new ApiResponse<string>(false, "A hint with this Id does not exist"));
            }

            DisplayHint displayHint = new DisplayHint(hint.HintText, hint.ImageUrl);
            return Ok(new ApiResponse<DisplayHint>(true, displayHint));
        }

        [HttpGet("gethintscenario/{scenarioId}/{hintId}")]
        public async Task<IActionResult> GetHintByScenarioAndHintId(int scenarioId, int hintId)
        {
            if (hintId < 1)
            {
                return BadRequest(new ApiResponse<string>(false, "A hint with this Id does not exist"));
            }

            var hint = await _dbContext.Hints.Where(x => x.HintId == hintId && x.ScenarioId == scenarioId).FirstOrDefaultAsync();
            if(hint == default)
            {
                return BadRequest(new ApiResponse<string>(false, "A hint with this Id does not exist"));
            }

            DisplayHint displayHint = new DisplayHint(hint.HintText, hint.ImageUrl);
            return Ok(new ApiResponse<DisplayHint>(true, displayHint));
        }

        [HttpGet("getflaghint/{teamId}/{flagId}")]
        public async Task<IActionResult> GetFlagHint(int teamId, int flagId)
        {
            if (teamId < 1 || !await _dbContext.Teams.AnyAsync(x => x.Id == teamId))
            {
                return BadRequest(new ApiResponse<string>(false, "No team exists with that Id"));
            }

            var team = await _dbContext.Teams.Where(x => x.Id == teamId).FirstOrDefaultAsync();
            var hint = await _dbContext.Hints.Where(x => x.FlagId == flagId).FirstOrDefaultAsync();

            if (hint == default)
            {
                return BadRequest(new ApiResponse<string>(false, "There is no hint that corresponds with the given flag"));
            }

            HintLog hintLog = new HintLog(team.Id, hint.HintId, hint.ScenarioId);
            if (!await _dbContext.HintLogs.Where(x => x.ScenarioId == hint.ScenarioId && x.HintId == hint.HintId && x.TeamId == team.Id).AnyAsync())
            {
                await _dbContext.HintLogs.AddAsync(hintLog);
            }

            await _dbContext.SaveChangesAsync();
            DisplayHint displayHint = new DisplayHint(hint.HintText, hint.ImageUrl);
            return Ok(new ApiResponse<DisplayHint>(true, displayHint));
        }

        [HttpGet("getpenalty/{teamId}/{scenarioId}")]
        public async Task<IActionResult> GetTimePenalty(int teamId, int scenarioId)
        {
            if (scenarioId < 1 || !await _dbContext.Scenarios.AnyAsync(x => x.Id == scenarioId))
            {
                return BadRequest(new ApiResponse<string>(false, "This scenario does not exist"));
            }

            List<int> PenaltyList = new List<int>();

            if (teamId < 1 || !await _dbContext.Teams.AnyAsync(x => x.Id == teamId))
            {
                return BadRequest(new ApiResponse<string>(false, "No team exists with that Id"));
            }

            var hintsUsed = (await _dbContext.HintLogs.Include(x => x.Hint).Where(x => x.TeamId == teamId).Select(u => u.Hint).ToListAsync());

            foreach (var hintUsed in hintsUsed)
            {
                var TimePenalty = await _dbContext.Hints.Where(x => x.HintId == hintUsed.HintId && scenarioId == x.ScenarioId).Select(u => u.TimePenalty).ToListAsync();
                PenaltyList.AddRange(TimePenalty);
            }

            if (!PenaltyList.Any())
            {
                return Ok(new ApiResponse<int>(true, 0));
            }
            else
            {
                int TimePenalty = PenaltyList.Sum(x => Convert.ToInt32(x));
                return Ok(new ApiResponse<int>(true, TimePenalty));
            }
        }
    }
}
