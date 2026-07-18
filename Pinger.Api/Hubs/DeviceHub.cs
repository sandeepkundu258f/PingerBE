using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Services;

namespace Pinger.Api.Hubs;

[Authorize]
public class DeviceHub(IDeviceHubService deviceHubService, SignalRSessionManager sessionManager): Hub
{
    public override async  Task OnConnectedAsync()
    {
        try
        {
            var userId = Context.UserIdentifier; //  maps to NameIdentifier claim (userid)
            var sessionId = Context.User?.FindFirst("SessionId")?.Value;
        
            if (sessionId != null && userId != null)
            {
                var session = await deviceHubService.ActivateSession(sessionId);
                if (session != null)
                {
                    sessionManager.RegisterSession(sessionId, Context);
                    await Clients.User(userId).SendAsync("UpdateDeviceList");
                }
            }
            await base.OnConnectedAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = Context.UserIdentifier;
            var sessionId = Context.User?.FindFirst("SessionId")?.Value;
        
            if (sessionId != null && userId != null)
            {
                var session = await deviceHubService.DeactivateSession(sessionId);
                if (session != null)
                {
                    sessionManager.UnregisterSession(sessionId);
                    await Clients.User(userId).SendAsync("UpdateDeviceList");
                }
            
            }
            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}