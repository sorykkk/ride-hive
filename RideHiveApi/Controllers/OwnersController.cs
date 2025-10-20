using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideHiveApi.Data;
using RideHiveApi.Models;

namespace RideHiveApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OwnersController> _logger;

        public OwnersController(AppDbContext context, ILogger<OwnersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Owners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Owner>>> GetAllOwners()
        {
            try
            {
                var owners = await _context.Owners.ToListAsync();
                return Ok(owners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving owners");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Owners
        [HttpPost]
        public async Task<ActionResult<Owner>> CreateOwner([FromBody] CreateOwnerDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var owner = new Owner
                {
                    OwnerId = dto.OwnerId,
                    Name = dto.Name
                };

                _context.Owners.Add(owner);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOwner), new { id = owner.OwnerId }, owner);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating owner");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Owners/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Owner>> GetOwner(string id)
        {
            try
            {
                var owner = await _context.Owners.FirstOrDefaultAsync(o => o.OwnerId == id);

                if (owner == null)
                    return NotFound($"Owner with ID {id} not found");

                return Ok(owner);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving owner with ID: {OwnerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class CreateOwnerDto
    {
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}