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
            return Ok(new ApiResponse<List<Flag>>(true, flags));
        }

        [HttpPost]
        public async Task<IActionResult> CompleteFlag(CompleteFlag completeFlag)
        {
            if (completeFlag == default)
            {
                return BadRequest(new ApiResponse<string>(false, "CompleteFlag cannot be empty!"));
            }

            if (completeFlag.TeamId < 0 || string.IsNullOrWhiteSpace(completeFlag.FlagCode))
            {
                return BadRequest(new ApiResponse<string>(false, "FlagId cannot be 0 or flagcode cannot be empty!"));
            }

            var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == completeFlag.TeamId);
            if (team == default)
            {
                return BadRequest(new ApiResponse<string>(false, "TeamId does not exist!"));
            }

            var flag = await _dbContext.Flags.FirstOrDefaultAsync(x => x.FlagCode == completeFlag.FlagCode);
            if (flag == default)
            {
                return BadRequest(new ApiResponse<string>(false, "Flag incorrect!"));
            }

            var startedScenario = await _dbContext.StartedScenarios.Where(x => x.Team.Id == team.Id && x.Scenario.Id == flag.ScenarioId).FirstOrDefaultAsync();
            if(startedScenario == default)
            {
                return BadRequest(new ApiResponse<string>(false, "Scenario is not started for this training yet!"));
            }

            if (startedScenario.EndTime != null)
            {
                return BadRequest(new ApiResponse<string>(false, "Team has already completed this scenario!"));
            }

            if (await _dbContext.FlagsCompleted.AnyAsync(x => x.Team.Id == team.Id && x.CompletedFlag.Id == flag.Id))
            {
                return BadRequest(new ApiResponse<string>(false, "Flag is already completed!"));
            }


            await _dbContext.FlagsCompleted.AddAsync(new FlagCompleted() { Team = team, CompletedFlag = flag });
            await _dbContext.SaveChangesAsync();

            var teamFlagStatus = new List<TeamFlagStatus>();

            foreach (var iFlag in await _dbContext.Flags.Where(x => x.ScenarioId == flag.ScenarioId).ToListAsync())
            {
                var strippedFlag = new TeamFlagStatus() { Flag = iFlag, IsCompleted = await _dbContext.FlagsCompleted.AnyAsync(x => x.Team.Id == team.Id && x.CompletedFlag.Id == iFlag.Id) };
                if(!strippedFlag.IsCompleted)
                {
                    strippedFlag.Flag.FlagCode = "";
                }

                teamFlagStatus.Add(strippedFlag);
            }
            return Ok(new ApiResponse<List<TeamFlagStatus>>(true, teamFlagStatus));
        }
    }
}
