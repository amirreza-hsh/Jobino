
using Jobino.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jobino.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string GenerateToken(User user)
    {
        var jwtSecret = configuration["Settings:JwtSecret"]!;
        var issuer = configuration["Settings:Issuer"]!;
        var audience = configuration["Settings:Audience"]!;

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString())
        };

        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(
                ClaimTypes.Role,
                userRole.Role.Name));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
