using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Infrastructure.Services;

public class SignalRSessionManager(IServiceScopeFactory scopeFactory)
{
    private readonly ConcurrentDictionary<string, HubCallerContext> _activeSessions = new();

    public void RegisterSession(string sessionId, HubCallerContext  hubCallerContext)
    {
        _activeSessions[sessionId] = hubCallerContext;
    }

    public void UnregisterSession(string sessionId)
    {
        _activeSessions.TryRemove(sessionId, out _);
    }

    public async Task<bool> KickSession(string sessionId)
    {
        if (_activeSessions.TryRemove(sessionId, out var callerContext))
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var userSession = await dbContext.UserSessions.SingleOrDefaultAsync(x => x.SessionId == Guid.Parse(sessionId));
                if (userSession != null)
                {
                    dbContext.UserSessions.Remove(userSession);
                    await dbContext.SaveChangesAsync();
                }
            }
            
            callerContext.Abort();
            return true;
        }
        return false;
    }
}