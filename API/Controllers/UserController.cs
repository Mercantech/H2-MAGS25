using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DomainModels;
using API.Data;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using API.Service;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

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

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> GetUsersWithOptions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool includeBookings = false,
            [FromQuery] int? limit = null,
            [FromQuery] string? sortBy = "name",
            [FromQuery] bool ascending = true)
        {
            // Debug logging
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine($"🔐 Authorization header: {authHeader}");
            
            if (User.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine($"✅ User authenticated: {User.Identity.Name}");
                Console.WriteLine($"Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
            }
            else
            {
                Console.WriteLine($"❌ User not authenticated");
            }

            // Validering af parametre
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            if (limit.HasValue && (limit < 1 || limit > 1000)) limit = 1000;

            // Start med base query
            var query = _context.Users.AsQueryable();

            // Søgning
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => 
                    u.Name.ToLower().Contains(search.ToLower()) ||
                    u.Email.ToLower().Contains(search.ToLower()));
            }

            // Indkludering af relaterede data
            if (includeBookings)
            {
                query = query.Include(u => u.BookingUsers)
                           .ThenInclude(bu => bu.Booking);
            }

            // Sortering
            query = sortBy.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name),
                "email" => ascending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                "createdat" => ascending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt),
                "updatedat" => ascending ? query.OrderBy(u => u.UpdatedAt) : query.OrderByDescending(u => u.UpdatedAt),
                _ => ascending ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name)
            };

            // Tæl total antal (før pagination)
            var totalCount = await query.CountAsync();

            // Pagination
            var skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            // Limit (hvis angivet)
            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            // Hent data
            var users = await query.ToListAsync();

            // Beregn pagination info
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var hasNextPage = page < totalPages;
            var hasPreviousPage = page > 1;

            // Returner resultat med metadata
            var result = new
            {
                Data = users,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasNextPage = hasNextPage,
                    HasPreviousPage = hasPreviousPage
                },
                Options = new
                {
                    Search = search,
                    IncludeBookings = includeBookings,
                    Limit = limit,
                    SortBy = sortBy,
                    Ascending = ascending
                }
            };

            return Ok(result);
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