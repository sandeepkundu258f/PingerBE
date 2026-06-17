using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pinger.Application.Domain;
using Pinger.Application.DTOs;
using Pinger.Application.Enums;
using Pinger.Application.Services.Interface;
using Pinger.Application.Utility;
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
            .SingleOrDefaultAsync(u => u.Username == request.Username && u.IsDeleted == false);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null; // Invalid credentials

        var jwtSettings = configuration.GetSection("JwtSettings");
        return AuthUtility.GenerateJwtToken(user, jwtSettings);
    }
}