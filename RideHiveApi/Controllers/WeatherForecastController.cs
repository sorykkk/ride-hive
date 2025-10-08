using Microsoft.AspNetCore.Mvc;
using RideHiveApi.Models;

namespace RideHiveApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("analyze")]
        public IActionResult AnalyzeTemperature([FromBody] TemperatureRequest request)
        {
            if (request?.Temperature == null)
            {
                return BadRequest("Temperature is required");
            }

            string adjective = GetTemperatureAdjective(request.Temperature);
            
            return Ok(new TemperatureResponse 
            { 
                Temperature = request.Temperature,
                Adjective = adjective,
                Message = $"It's {adjective} at {request.Temperature}°C"
            });
        }

        private string GetTemperatureAdjective(double temperature)
        {
            return temperature switch
            {
                <= -10 => "Freezing",
                <= 0 => "Very Cold",
                <= 10 => "Cold",
                <= 15 => "Cool",
                <= 20 => "Mild",
                <= 25 => "Warm",
                <= 30 => "Hot",
                <= 35 => "Very Hot",
                _ => "Scorching"
            };
        }
    }
}
