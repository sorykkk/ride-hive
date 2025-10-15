using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RideHiveApi.Models;
using RideHiveApi.Models.DataTransferObjects;

namespace RideHiveApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<UserController> logger;

        public UserController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<UserController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        // POST: api/account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check if email exist
            var existingUser = await this.userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email already exists" });

            //Create user
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname,
                RegisteredAt = DateTime.UtcNow,
                Age = model.Age,
                Phone = model.Phone
            };

            //If client exist with driving license
            if (model.Role == "Client" && model.DrivingLicenseImage != null)
            {
                // Photo (max 5MB)
                if (model.DrivingLicenseImage.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Image size cannot exceed 5MB" });
                }

                // TYPE of photo
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(model.DrivingLicenseImage.ContentType.ToLower()))
                {
                    return BadRequest(new { message = "Only JPG, JPEG, and PNG images are allowed" });
                }

                using (var memoryStream = new MemoryStream())
                {
                    await model.DrivingLicenseImage.CopyToAsync(memoryStream);
                    user.ImageDrivingLicense = memoryStream.ToArray();
                }
            }

            
            var result = await this.userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            // Add role
            await this.userManager.AddToRoleAsync(user, model.Role);

            this.logger.LogInformation($"New user registered: {user.Email} as {model.Role}");

            return Ok(new
            {
                message = "Registration successful",
                userId = user.Id,
                email = user.Email,
                role = model.Role
            });
        }

        // POST: api/account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            //Check if email exist
            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var result = await this.signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var roles = await this.userManager.GetRolesAsync(user);

                this.logger.LogInformation($"User logged in: {user.Email}");

                return Ok(new UserAuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = roles.FirstOrDefault() ?? "Client",
                    RegisteredAt = user.RegisteredAt
                });
            }

            if (result.IsLockedOut)
            {
                return Unauthorized(new { message = "Account is locked due to multiple failed login attempts. Try again later." });
            }

            return Unauthorized(new { message = "Invalid email or password" });
        }

        // POST: api/account/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }

        // GET: api/account/test (Test)
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Account controller is working!" });
        }
    }
}