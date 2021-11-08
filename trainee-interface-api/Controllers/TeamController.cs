using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trainee_interface_api.Contexts;
using trainee_interface_api.Models;

namespace trainee_interface_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TeamController: ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public TeamController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            var result = await _dbContext.Teams.ToListAsync();
            return Ok(result);
        }

        [HttpGet("results")]
        public async Task<IActionResult> GetTeamResults()
        {
            var results = await _dbContext.FlagsCompleted.ToListAsync();
            foreach (var result in results)
            {
                result.CompletedFlag.FlagCode = "";
            }
            return Ok(results);
        }

        [HttpGet("results/{teamId}")]
        public async Task<IActionResult> GetTeamResultsByTeamId(int teamId)
        {
            var results = await _dbContext.FlagsCompleted.Where(x => x.Team.Id == teamId).ToListAsync();
            foreach (var result in results)
            {
                result.CompletedFlag.FlagCode = "";
            }
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam(Team team)
        {
            if(team == default)
            {
                return BadRequest();
            }

            if(string.IsNullOrEmpty(team.Name))
            {
                return BadRequest();
            }

            if(await _dbContext.Teams.AnyAsync(x => x.Name == team.Name))
            {
                return BadRequest();
            }

            await _dbContext.AddAsync(team);
            await _dbContext.SaveChangesAsync();

            return Ok(team);
        }
    }
}
