using Microsoft.EntityFrameworkCore;
using Counter.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add PostgreSQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();

// Allow CORS (Cross-Origin Resource Sharing)
// enables same origin policy (server explicitly allows browers send requests from one domain to another domain)
// frontend communicates with backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", 
        policy => policy
            //.AllowAnyOrigin()
            // sets which frontend URLs are allowed
            .WithOrigins("http://localhost:5173") // vue dev server
            // Allows sending headers like Content-Type or Authorization
            .AllowAnyHeader()
            // Allows GET, POST, PUT, DELETE
            .AllowAnyMethod());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
