//use:
// nswag openapi2tsclient /input:http://localhost:5030/swagger/v1/swagger.json /output:src/api/client.ts
// for generating clien.ts with all api

using RideHiveApi.Models.Settings;
using RideHiveApi.Data;
using RideHiveApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Entity Framework with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Configurare Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>() ?? new CorsSettings();
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get <ApiSettings> ()?? new ApiSettings();

// Allow CORS (Cross-Origin Resource Sharing)
// enables same origin policy (server explicitly allows browers send requests from one domain to another domain)
// frontend communicates with backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", 
        policy => policy
            //.AllowAnyOrigin()
            // sets which frontend URLs are allowed
            .WithOrigins(corsSettings.AllowedOrigins) // vue dev server
            // Allows sending headers like Content-Type or Authorization
            .AllowAnyHeader()
            // Allows GET, POST, PUT, DELETE
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedRoles(scope.ServiceProvider);
}

// Apply pending migrations on startup (for development)
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     context.Database.Migrate(); // This applies any pending migrations
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Only use HTTPS redirection in production
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
