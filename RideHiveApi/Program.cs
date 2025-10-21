using RideHiveApi.Models.Settings;
using RideHiveApi.Data;
using RideHiveApi.Models;
using RideHiveApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Entity Framework with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Configuration for password
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Configuration for user
    options.User.RequireUniqueEmail = true;

    // Configuration for lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

//Configure Cookie Authentication pentru cross-origin
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // None pentru development (HTTP)
    options.Cookie.SameSite = SameSiteMode.Lax; // Lax pentru cross-origin
    options.Cookie.Name = "RideHive.Auth";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    
    // Return 401 instead of redirect pentru API
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

// Configurare Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Load CORS and API settings
// Register image upload service
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>() ?? new CorsSettings();
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", 
        policy => policy
            .WithOrigins(corsSettings.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply pending migrations on startup (for development)
// Commands for Entity Framework:
//      dotnet ef migrations add <MigrationName>
//      dotnet ef database update
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     context.Database.Migrate(); // This applies any pending migrations
// }

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedRoles(scope.ServiceProvider);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Only use HTTPS redirection in production
    app.UseHttpsRedirection();
}

//CORS must be before Authentication/Authorization
app.UseCors("AllowFrontend");

app.UseAuthentication();
// Serve static files (images)
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();