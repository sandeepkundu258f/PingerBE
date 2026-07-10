using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinger.Application.DTOs;
using Pinger.Application.DTOs.DeviceHubDTOs;
using Pinger.Application.Services.Interface;

namespace Pinger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeviceHubController(IDeviceHubService deviceHubService): ControllerBase
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
}