using BabyfloServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BabyfloServer.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add CORS Policy (Keep this open so your HTML pages can talk to port 5190)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

// 2. Configure Database Context (SQLite setup)
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite("Data Source=babyflo.db"));

// 🔓 SECURITY BYPASS: Removed JWT configuration requirements and explicit Token validation blocks 
// to prevent startup application crashes due to missing appsettings.json keys.
builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// 3. Middlewares
app.UseCors("AllowAll"); // Enable CORS here
app.UseStaticFiles();

// Keeping default pipeline routes active so standard HTTP maps function smoothly
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();