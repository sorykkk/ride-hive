using Microsoft.AspNetCore.Mvc;

namespace RideHiveApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                message = "Welcome to RideHive API", 
                status = "Running",
                timestamp = DateTime.UtcNow 
            });
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { 
                service = "RideHive API",
                version = "1.0.0",
                status = "Healthy",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
