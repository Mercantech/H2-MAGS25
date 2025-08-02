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
    private readonly int? _expiresValue;

    public JWTService(IOptions<JWTSettings> options)
    {
        _settings = options.Value;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _settings.Secret ?? Environment.GetEnvironmentVariable("JWT_SECRET") ?? "default-secret-key";
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            }),
            Expires = DateTime.UtcNow.AddMinutes(60 + 120),
            Issuer = _settings.Issuer ?? Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MAGSLearn",
            Audience = _settings.Audience ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MAGSLearn",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
