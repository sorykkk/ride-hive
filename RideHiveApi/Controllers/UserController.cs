using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideHiveApi.Data;
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
        private readonly AppDbContext dbContext;

        public UserController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<UserController> logger,
            AppDbContext dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.dbContext = dbContext;
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

            // If Client role, driving license is mandatory
            if (model.Role == "Client" && model.DrivingLicenseImage == null)
            {
                return BadRequest(new { message = "Driving license image is required for client registration" });
            }

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

            //If client with driving license
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

            // If Owner role, create Owner entity
            if (model.Role == "Owner")
            {
                var owner = new Owner
                {
                    OwnerId = user.Id,
                    Name = $"{user.Name} {user.Surname}"
                };
                await this.dbContext.Owners.AddAsync(owner);
                await this.dbContext.SaveChangesAsync();
            }

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

            var result = await this.signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: true);

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
                    RegisteredAt = user.RegisteredAt,
                    Phone = user.Phone,
                    Age = user.Age,
                    Bio = user.Bio,
                    Location = user.Location,
                    HasProfileImage = user.ProfileImage != null
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

        // PUT: api/user/profile
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = this.userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Update user properties
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Phone = model.Phone;
            user.Age = model.Age;
            user.Bio = model.Bio;
            user.Location = model.Location;

            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            var roles = await this.userManager.GetRolesAsync(user);

            this.logger.LogInformation($"User profile updated: {user.Email}");

            return Ok(new
            {
                message = "Profile updated successfully",
                user = new UserAuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = roles.FirstOrDefault() ?? "Client",
                    RegisteredAt = user.RegisteredAt,
                    Phone = user.Phone,
                    Age = user.Age,
                    Bio = user.Bio,
                    Location = user.Location,
                    HasProfileImage = user.ProfileImage != null
                }
            });
        }

        // POST: api/user/profile/image
        [Authorize]
        [HttpPost("profile/image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { message = "No image provided" });

            var userId = this.userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Validate image size (max 5MB)
            if (image.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Image size cannot exceed 5MB" });

            // Validate image type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(image.ContentType.ToLower()))
                return BadRequest(new { message = "Only JPG, PNG, and GIF images are allowed" });

            // Convert image to byte array
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                user.ProfileImage = memoryStream.ToArray();
                user.ProfileImageContentType = image.ContentType;
            }

            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            this.logger.LogInformation($"Profile image uploaded for user: {user.Email}");

            return Ok(new { message = "Profile image uploaded successfully" });
        }

        // GET: api/user/profile/image/{userId}
        [HttpGet("profile/image/{userId}")]
        public async Task<IActionResult> GetProfileImage(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null || user.ProfileImage == null)
                return NotFound();

            return File(user.ProfileImage, user.ProfileImageContentType ?? "image/jpeg");
        }

        // DELETE: api/user/profile/image
        [Authorize]
        [HttpDelete("profile/image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var userId = this.userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.ProfileImage = null;
            user.ProfileImageContentType = null;

            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            this.logger.LogInformation($"Profile image deleted for user: {user.Email}");

            return Ok(new { message = "Profile image deleted successfully" });
        }

        // GET: api/user/basic-info/{userId} - Get basic user information
        [HttpGet("basic-info/{userId}")]
        public async Task<IActionResult> GetBasicUserInfo(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                userId = user.Id,
                name = user.Name,
                surname = user.Surname,
                fullName = $"{user.Name} {user.Surname}",
                role = (await this.userManager.GetRolesAsync(user)).FirstOrDefault(),
                hasProfileImage = user.ProfileImage != null
            });
        }

        // POST: api/user/sync-owners - Sync existing Owner users
        [HttpPost("sync-owners")]
        public async Task<IActionResult> SyncOwners()
        {
            var ownerUsers = await this.userManager.GetUsersInRoleAsync("Owner");
            var existingOwnerIds = await this.dbContext.Owners.Select(o => o.OwnerId).ToListAsync();

            int created = 0;
            foreach (var user in ownerUsers)
            {
                if (!existingOwnerIds.Contains(user.Id))
                {
                    var owner = new Owner
                    {
                        OwnerId = user.Id,
                        Name = $"{user.Name} {user.Surname}"
                    };
                    await this.dbContext.Owners.AddAsync(owner);
                    created++;
                }
            }

            if (created > 0)
            {
                await this.dbContext.SaveChangesAsync();
            }

            return Ok(new { message = $"Synced {created} owner records" });
        }
    }
}