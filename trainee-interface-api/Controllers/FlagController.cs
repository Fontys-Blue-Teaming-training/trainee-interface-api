using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trainee_interface_api.Contexts;
using trainee_interface_api.Models;
using trainee_interface_api.Models.DTO;

namespace trainee_interface_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FlagController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public FlagController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{scenarioId}")]
        public async Task<IActionResult> GetFlagsByScenarioId(int scenarioId)
        {
            var flags = await _dbContext.Flags.Include(x => x.Scenario).Where(x => x.ScenarioId == scenarioId).ToListAsync();
            foreach (var flag in flags)
            {
                flag.FlagCode = "";
            }
            return Ok(flags);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteFlag(CompleteFlag completeFlag)
        {
            if (completeFlag == default)
            {
                return BadRequest();
            }

            if (completeFlag.TeamId < 0 || string.IsNullOrWhiteSpace(completeFlag.FlagCode))
            {
                return BadRequest();
            }

            var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == completeFlag.TeamId);
            if (team == default)
            {
                return BadRequest();
            }

            var flag = await _dbContext.Flags.FirstOrDefaultAsync(x => x.FlagCode == completeFlag.FlagCode);
            if (flag == default)
            {
                return BadRequest();
            }

            if (await _dbContext.FlagsCompleted.AnyAsync(x => x.Team.Id == team.Id && x.CompletedFlag.Id == flag.Id))
            {
                return BadRequest();
            }

            await _dbContext.FlagsCompleted.AddAsync(new FlagCompleted() { Team = team, CompletedFlag = flag });
            await _dbContext.SaveChangesAsync();

            var teamFlagStatus = new List<TeamFlagStatus>();

            foreach (var iFlag in _dbContext.Flags.Where(x => x.ScenarioId == flag.ScenarioId))
            {
                var strippedFlag = new TeamFlagStatus() { Flag = iFlag, IsCompleted = await _dbContext.FlagsCompleted.AnyAsync(x => x.Team.Id == team.Id && x.CompletedFlag.Id == flag.Id) };
                if(!strippedFlag.IsCompleted)
                {
                    strippedFlag.Flag.FlagCode = "";
                }

                teamFlagStatus.Add(strippedFlag);
            }

            return Ok(teamFlagStatus);
        }
    }
}
