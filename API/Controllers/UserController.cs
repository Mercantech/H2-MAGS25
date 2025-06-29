using Microsoft.AspNetCore.Mvc;
using DomainModels;
using API.Data;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using API.Service;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JWTService _jwtService;

        public UserController(AppDbContext context, JWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var normalizedEmail = request.Email.ToLowerInvariant();
            var normalizedName = request.Name.ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Name == normalizedName))
                return BadRequest("Brugernavn er allerede taget.");
            if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
                return BadRequest("Email er allerede i brug.");

            var user = new User
            {
                Name = normalizedName,
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Bruger oprettet!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var normalizedEmail = request.Email.ToLowerInvariant();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
            if (user == null)
                return Unauthorized("Bruger findes ikke.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Forkert adgangskode.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Navn er påkrævet.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email er påkrævet.")]
        [EmailAddress(ErrorMessage = "Ugyldig email-adresse.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Adgangskode er påkrævet.")]
        [MinLength(6, ErrorMessage = "Adgangskode skal være mindst 6 tegn.")]
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email er påkrævet.")]
        [EmailAddress(ErrorMessage = "Ugyldig email-adresse.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Adgangskode er påkrævet.")]
        public string Password { get; set; }
    }
} 