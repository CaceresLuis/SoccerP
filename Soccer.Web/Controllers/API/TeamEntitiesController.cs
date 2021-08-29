using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamEntitiesController : ControllerBase
    {
        private readonly DataContext _context;

        public TeamEntitiesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/TeamEntities
        [HttpGet]
        public IEnumerable<TeamEntity> GetTeams()
        {
            return _context.Teams.OrderBy(t => t.Name);
        }

        // GET: api/TeamEntities/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            TeamEntity teamEntity = await _context.Teams.FindAsync(id);

            if (teamEntity == null) return NotFound();
            return Ok(teamEntity);
        }
    }
}