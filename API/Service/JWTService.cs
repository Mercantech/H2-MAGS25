using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DomainModels;
using Microsoft.Extensions.Options;

namespace API.Service;

public class JWTSettings
{
    public string Secret { get; set; }
    public int ExpiryMinutes { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}

public class JWTService
{
    private readonly JWTSettings _settings;
    private readonly int _expiresValue;

    public JWTService(IOptions<JWTSettings> options)
    {
        _settings = options.Value;
        _expiresValue = _settings.ExpiryMinutes ?? int.Parse(GetEnvironmentVariable("JWT_EXPIRY_MINUTES"));
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Secret) ?? GetEnvironmentVariable("JWT_SECRET");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            }),
            Expires = DateTime.UtcNow.AddMinutes(_expiresValue + 120),
            Issuer = _settings.Issuer ?? GetEnvironmentVariable("JWT_ISSUER"),
            Audience = _settings.Audience ?? GetEnvironmentVariable("JWT_AUDIENCE"),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
