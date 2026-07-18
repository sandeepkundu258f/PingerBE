using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinger.Application.DTOs;
using Pinger.Application.DTOs.DeviceHubDTOs;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Services;

namespace Pinger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeviceHubController(IDeviceHubService deviceHubService, SignalRSessionManager sessionManager): ControllerBase
{
    [ProducesResponseType(typeof(IEnumerable<DeviceListResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status401Unauthorized)]
    [HttpGet("DeviceList")]
    public async Task<IActionResult> GetDeviceList()
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdString))
            {
                var userId = int.Parse(userIdString);
                var deviceList = await deviceHubService.ListDevices(userId);
                return Ok(deviceList);
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new StandardResponseDto("Invalid user"));
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status404NotFound)]
    [HttpDelete("{sessionId}/TerminateSession")]
    public async Task<IActionResult> TerminateSession([FromRoute] string sessionId)
    {
        try
        {
            bool wasKicked = await sessionManager.KickSession(sessionId);
            
            if (wasKicked)
            {
                return StatusCode(StatusCodes.Status200OK, new StandardResponseDto($"SignalR connection for session {sessionId} aborted."));
            }
            
            return StatusCode(StatusCodes.Status404NotFound, new StandardResponseDto("No active SignalR connection found for this session."));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}