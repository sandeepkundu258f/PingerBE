using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pinger.Application.Domain;
using Pinger.Application.DTOs.AuthDTOs;
using Pinger.Application.Enums;
using Pinger.Application.Services.Interface;
using Pinger.Application.Utility;
using Pinger.Infrastructure.Persistence;
using UAParser;

namespace Pinger.Infrastructure.Services;

public class AuthService(AppDbContext dbContext, IConfiguration configuration) : IAuthService
{
    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        try
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
        
            var requestedRoleIds = request.RoleIds != null && request.RoleIds.Count !=0 ? request.RoleIds: [(int)RoleEnum.User];

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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    public async Task<string?> LoginAsync(LoginRequestDto loginRequest,  HttpRequest httpRequest)
    {
        try
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .SingleOrDefaultAsync(u => u.Username == loginRequest.Username && u.IsDeleted == false);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                return null; // Invalid credentials
            
            var sessionId = await GenerateSessionId(user.Id, loginRequest.DeviceIdentifier, httpRequest);
            
            var jwtSettings = configuration.GetSection("JwtSettings");
            return AuthUtility.GenerateJwtToken(user, jwtSettings, sessionId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<Guid> GenerateSessionId(int userId, Guid deviceIdentifier, HttpRequest httpRequest)
    {
        var userAgent = httpRequest.Headers["User-Agent"].ToString(); //for browser
        var clientDeviceName = httpRequest.Headers["X-Device-Name"].ToString(); //for phone
            
        var uaParser = Parser.GetDefault();
        var clientInfo = uaParser.Parse(userAgent);
            
        var finalDeviceName = !string.IsNullOrEmpty(clientDeviceName) 
            ? clientDeviceName 
            : $"{clientInfo.UA.Family} on {clientInfo.OS.Family}";
            
        Guid sessionId;
        var oldSession = await dbContext.UserSessions
            .SingleOrDefaultAsync(x=>x.UserId == userId && x.DeviceIdentifier == deviceIdentifier);
        if (oldSession != null)
        {
            sessionId =  oldSession.SessionId;
            oldSession.DeviceName = finalDeviceName;
            oldSession.IpAddress = httpRequest.HttpContext.Connection.RemoteIpAddress.ToString();
            await dbContext.SaveChangesAsync();
        }
        else
        {
            var session = new UserSession
            {
                UserId = userId,
                DeviceName = finalDeviceName,
                DeviceIdentifier = deviceIdentifier,
                IpAddress = httpRequest.HttpContext.Connection.RemoteIpAddress.ToString(),
            };
            
            dbContext.UserSessions.Add(session);
            await dbContext.SaveChangesAsync();
            sessionId = session.SessionId;
        }
        
        return sessionId;
    }
}