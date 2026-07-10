using Microsoft.EntityFrameworkCore;
using Pinger.Application.Domain;
using Pinger.Application.DTOs.DeviceHubDTOs;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Infrastructure.Services;

public class DeviceHubService(AppDbContext dbContext) : IDeviceHubService
{
    public async Task<UserSession?> ActivateSession(string sessionId)
    {
        return await ChangeUserSessionState(sessionId, true);
    }

    public async Task<UserSession?> DeactivateSession(string sessionId)
    {
        return await ChangeUserSessionState(sessionId, false);
    }

    public async Task<IEnumerable<DeviceListResponseDto>> ListDevices(int targetUserId)
    {
        try
        {
            return await dbContext.UserSessions
                .Where(x => x.UserId == targetUserId)
                .Select(x => new DeviceListResponseDto
                {
                    SessionId = x.SessionId,
                    LastActiveAt = x.LastActiveAt,
                    IsOnline = x.IsOnline,
                    IpAddress = x.IpAddress,
                    DeviceName = x.DeviceName,
                    DeviceIdentifier = x.DeviceIdentifier
                })
                .ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<UserSession?> ChangeUserSessionState(string sessionId, bool isOnline)
    {
        try
        {
            var sessionGuid = Guid.Parse(sessionId);
            var userSession = await dbContext.UserSessions.SingleOrDefaultAsync(x => x.SessionId == sessionGuid);
            if (userSession is not null)
            {
                userSession.IsOnline = isOnline;
                userSession.LastActiveAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
            }
            return userSession;
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}