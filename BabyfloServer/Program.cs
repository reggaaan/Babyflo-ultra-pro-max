using BabyfloServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BabyfloServer.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

// 2. Configure Database Context
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite("Data Source=babyflo.db"));

builder.Services.AddAuthorization();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// --- SELF-MIGRATION BLOCK (Add this) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        // This ensures the database is created and migrations are applied
        context.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration Error: {ex.Message}");
    }
}
// ---------------------------------------

// 3. Middlewares
app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
