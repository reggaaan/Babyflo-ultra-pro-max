using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BabyfloServer.Data;
using BabyfloServer.Models;

namespace BabyfloServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(DataContext context, IConfiguration config, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email))
                return BadRequest(new { message = "Email already registered." });

            var user = new User
            {
                Name = (req.Name ?? string.Empty).Trim(),
                Email = email,
                Role = string.IsNullOrWhiteSpace(req.Role) ? "User" : req.Role
            };

            user.Password = _passwordHasher.HashPassword(user, req.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registered" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (user == null) return Unauthorized(new { message = "Invalid credentials." });

            var verify = _passwordHasher.VerifyHashedPassword(user, user.Password, req.Password);
            if (verify != PasswordVerificationResult.Success)
                return Unauthorized(new { message = "Invalid credentials." });

            var role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role;
            bool isAdminUser = (role == "Admin" || user.IsAdmin);

            // 🔓 SECURITY BYPASS: Return a dummy token so the frontend variables don't break, 
            // completely avoiding the need for JWT configuration keys.
            var dummyToken = "thesis-demo-bypass-token";

            return Ok(new { 
                name = user.Name, 
                email = user.Email, 
                isAdmin = isAdminUser, 
                token = dummyToken 
            });
        }

        [HttpGet("seed-customer")]
        public async Task<IActionResult> SeedCustomer()
        {
            var email = "maria@gmail.com";
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
                return Ok("Customer already exists.");

            var user = new User { Name = "Maria Santos", Email = email, Role = "Customer" };
            user.Password = _passwordHasher.HashPassword(user, "password123");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Customer Maria seeded successfully!");
        }
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}