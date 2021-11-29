using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using trainee_interface_api.Contexts;
using trainee_interface_api.Models.DTO;

namespace trainee_interface_api.Controllers
{
    [Route("[controller]")]
    [ApiController]


    public class HintController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public HintController(DatabaseContext dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet("/{HintId}")]
        public async Task<IActionResult> GetHintByHintId(int hintId)
        {
            var hint = await _dbContext.Hints.Where(x => x.HintId == hintId).FirstOrDefaultAsync();
            DisplayHint displayHint = new DisplayHint(hint.HintText, hint.ImageUrl);
            return Ok(new ApiResponse<DisplayHint>(true, displayHint));
        }

        [HttpGet("/{ScenarioId}/{HintId}")]
        public async Task<IActionResult> GetHintByScenarioAndHintId(int scenarioId, int hintId)
        {
            var hint = await _dbContext.Hints.Where(x => x.HintId == hintId && x.ScenarioId == scenarioId).FirstOrDefaultAsync();
            DisplayHint displayHint = new DisplayHint(hint.HintText, hint.ImageUrl);
            return Ok(new ApiResponse<DisplayHint>(true, displayHint));
        }

    }
}
