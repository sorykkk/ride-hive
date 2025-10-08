using RideHiveApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Bind configuration to models
var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>() ?? new CorsSettings();
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();

// Register configuration objects for DI
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Configure CORS using configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", 
        policy => policy
            .WithOrigins(corsSettings.AllowedOrigins)
            .AllowAnyHeader()
            // Allows GET, POST, PUT, DELETE
            .AllowAnyMethod());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline (order matters)
app.UseCors("AllowFrontend");

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
