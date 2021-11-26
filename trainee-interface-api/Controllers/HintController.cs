using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using trainee_interface_api.Contexts;

namespace trainee_interface_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class HintController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public HintController(DatabaseContext dbContext)
        {
            dbContext = _dbContext;
        }

    }
}
