using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pinger.Application.Domain;
using Pinger.Application.DTOs;
using Pinger.Application.Enums;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Infrastructure.Services;

public class AuthService(AppDbContext dbContext, IConfiguration configuration) : IAuthService
{
    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        if (await dbContext.Users.AnyAsync(u => u.Username == request.Username))
            return false; // Username taken

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(); // Generates the User ID
        
        var requestedRoleIds = request.RoleIds ?? [(int)RoleEnum.User];

        foreach (var requestedRoleId in requestedRoleIds.Where(roleId => Enum.IsDefined(typeof(RoleEnum), roleId)))
        {
            dbContext.UserRoles.Add(new UserRole 
            { 
                UserId = user.Id, 
                RoleId = requestedRoleId
            });
        }
        
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(LoginRequestDto request)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(ur => ur.UserRoles)
            .ThenInclude(r => r.Role)
            .SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null; // Invalid credentials

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key missing"));

        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        ];

        foreach (var userRole in user.UserRoles)
        {
            if (userRole.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}