﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        
        [HttpGet("highscore/{scenarioId}")]
        public async Task<IActionResult> GetHighscoreByScenarioId(int scenarioId)
        {
            if(scenarioId < 1)
            {
                return BadRequest(new ApiResponse<string>(false, "ScenarioId is incorrect"));
            }

            if(!await _dbContext.Scenarios.AnyAsync(x => x.Id == scenarioId))
            {
                return BadRequest(new ApiResponse<string>(false, "ScenarioId does not exist"));
            }

            var highscore = new List<HighscoreEntry>();

            foreach (var completedFlag in await _dbContext.FlagsCompleted.Include(x => x.CompletedFlag).Include(x => x.Team).Where(x => x.CompletedFlag.ScenarioId == scenarioId).ToListAsync())
            {
                var teamIndex = highscore.FindIndex(x => x.TeamName == completedFlag.Team.Name);
                if (teamIndex < 0)
                {
                    highscore.Add(new HighscoreEntry()
                    {
                        TeamName = completedFlag.Team.Name,
                        Points = completedFlag.CompletedFlag.Points,
                        AmountOfFlags = 1
                    });
                }
                else
                {
                    highscore[teamIndex].AmountOfFlags++;
                    highscore[teamIndex].Points += completedFlag.CompletedFlag.Points;
                }
            }

            highscore = highscore.OrderBy(x => x.Points).ToList();

            return Ok(new ApiResponse<List<HighscoreEntry>>(true, highscore));
        }

        [HttpGet("leaderboard/{scenarioId}")]
        public async Task<IActionResult> GetLeaderboardByScenarioId(int scenarioId)
        {
            if (scenarioId < 1)
            {
                return BadRequest(new ApiResponse<string>(false, "ScenarioId is incorrect"));
            }

            if (!await _dbContext.Scenarios.AnyAsync(x => x.Id == scenarioId))
            {
                return BadRequest(new ApiResponse<string>(false, "ScenarioId does not exist"));
            }

            var leaderboard = new List<LeaderboardEntry>();
            foreach (var completedFlag in await _dbContext.FlagsCompleted.Include(x => x.CompletedFlag).Include(x => x.Team).Where(x => x.CompletedFlag.ScenarioId == scenarioId).ToListAsync())
            {
                var startScenario = await _dbContext.StartedScenarios.Where(x => x.Team.Id == completedFlag.Team.Id && x.Scenario.Id == completedFlag.CompletedFlag.ScenarioId).FirstOrDefaultAsync();
                leaderboard.Add(new LeaderboardEntry()
                {
                    TotalSeconds = (completedFlag.Completed - startScenario.StartTime).TotalSeconds,
                    FlagId = completedFlag.CompletedFlag.Id,
                    TeamName = completedFlag.Team.Name,
                    Points = completedFlag.CompletedFlag.Points,
                    TeamId = completedFlag.Team.Id
                });
            }
         
            return Ok(new ApiResponse<List<LeaderboardEntry>>(true, leaderboard));
        }

        [HttpGet("current/{teamId}")]
        public async Task<IActionResult> GetCurrentScenario(int teamId)
        {
            if(teamId < 1)
            {
                return BadRequest(new ApiResponse<string>(false, "teamId incorrect"));
            }

            if(await _dbContext.Teams.AnyAsync(x => x.Id == teamId))
            {
                return BadRequest(new ApiResponse<string>(false, "teamId doesnt exist"));
            }

            var currentScenario = await _dbContext.StartedScenarios.Include(x => x.Scenario).FirstOrDefaultAsync(x => x.Team.Id == teamId && x.EndTime == null);
            if(currentScenario == default)
            {
                return BadRequest(new ApiResponse<string>(false, "Team has no scenario that is in progress"));
            }

            return Ok(new ApiResponse<StartedScenario>(true,  currentScenario));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleScenario(ToggleScenario toggleScenario)
        {
            if (toggleScenario == default)
            {
                return BadRequest(new ApiResponse<string>(false, "ToggleScenario cannot be empty"));
            }

            var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == toggleScenario.TeamId);
            if (team == default)
            {
                return BadRequest(new ApiResponse<string>(false, "TeamId does not exist"));
            }

            var scenario = await _dbContext.Scenarios.FirstOrDefaultAsync(x => x.Id == toggleScenario.ScenarioId);
            if (scenario == default)
            {
                return BadRequest(new ApiResponse<string>(false, "ScenarioId does not exist"));
            }

            if (await _dbContext.StartedScenarios.Include(x => x.Scenario).AnyAsync(x => x.Scenario.Id != toggleScenario.ScenarioId && x.EndTime == null))
            {
                return BadRequest(new ApiResponse<string>(false, "Team already has a started scenario!"));
            }

            var startedScenario = await _dbContext.StartedScenarios.Where(x => x.Team.Id == toggleScenario.TeamId && x.Scenario.Id == toggleScenario.ScenarioId).FirstOrDefaultAsync();
            if(startedScenario != default)
            {
                if(startedScenario.EndTime != null)
                {
                    return BadRequest(new ApiResponse<string>(false, "Team already completed this scenario!"));
                }

                startedScenario.EndTime = DateTime.Now;
            }
            else
            {
                startedScenario = new StartedScenario()
                {
                    Team = team,
                    Scenario = scenario
                };

                await _dbContext.StartedScenarios.AddAsync(startedScenario);
            }

            await _dbContext.SaveChangesAsync();
            return Ok(new ApiResponse<StartedScenario>(true, startedScenario));
        }
    }
}
