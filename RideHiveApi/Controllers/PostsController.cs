using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideHiveApi.Models.DataTransferObjects;
using RideHiveApi.Data;
using RideHiveApi.Models;
using RideHiveApi.Models.Enums;
using RideHiveApi.Services;

namespace RideHiveApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PostsController> _logger;

        public PostsController(AppDbContext context, ILogger<PostsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Posts
        // It will get all posts sorted by posted date
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetAllPosts()
        {
            try
            {
                var posts = await _context.PostItems
                    .OrderByDescending(p => p.PostedAt)
                    .ThenByDescending(p => p.PostId)
                    .ToListAsync();

                var response = posts.Select(PostResponseDto.FromPostItem).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all posts");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PostResponseDto>> GetPost(int id)
        {
            try
            {
                var post = await _context.PostItems.FirstOrDefaultAsync(p => p.PostId == id);

                if (post == null)
                    return NotFound($"Post with ID {id} not found");

                var response = PostResponseDto.FromPostItem(post);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving post with ID: {PostId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Posts/owner/{ownerId}
        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetPostsByOwner(string ownerId)
        {
            try
            {
                var posts = await _context.PostItems
                    .Where(p => p.OwnerId.Equals(ownerId))
                    .ToListAsync();

                var response = posts.Select(PostResponseDto.FromPostItem).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving posts for owner: {OwnerId}", ownerId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Posts
        [HttpPost]
        public async Task<ActionResult<PostItem>> CreatePost([FromForm] PostCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var post = new PostItem
                {
                    OwnerId = dto.OwnerId,
                    CarId = dto.CarId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Price = dto.Price,
                    SpecialRequirements = dto.SpecialRequirements,
                    Location = dto.Location,
                    AvailableTimeSlots = dto.AvailableTimeSlots
                        .Select(timeSlot => DateTime.Parse(timeSlot))
                        .ToList(),
                    PostedAt = DateTime.UtcNow, // set server-side
                    Available = true // is available on creation
                };

                _context.PostItems.Add(post);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPost), new { id = post.PostId }, PostResponseDto.FromPostItem(post));
            }
            catch (FormatException)
            {
                return BadRequest("Invalid date format. Please use ISO 8601 format (e.g., 2025-10-19T14:30:00Z)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Posts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var post = await _context.PostItems
                    .FirstOrDefaultAsync(p => p.PostId == id);

                if (post == null)
                    return NotFound($"Post with ID {id} not found");

                // Remove from database
                _context.PostItems.Remove(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully deleted car ID: {id}");
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting post with ID: {PostId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}