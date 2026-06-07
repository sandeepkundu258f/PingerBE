using Microsoft.AspNetCore.Mvc;
using Pinger.Application.DTOs;
using Pinger.Application.Services.Interface;

namespace Pinger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TargetsController(IPingTargetService pingTargetService) : ControllerBase
{
    [HttpGet("GetAllTargets")]
    public async Task<IActionResult> GetTargets()
    {
        var targets = await pingTargetService.GetAllPingTargetsAsync();
        return Ok(targets);
    }

    [HttpPost("CreatePingTarget")]
    public async Task<IActionResult> CreateTarget([FromBody] CreatePingTargetRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("Name and URL are required.");
        }
        
        var newTarget = await pingTargetService.CreatePingTargetAsync(
            request.Name,
            request.Url,
            request.IntervalSeconds
        );
        
        return CreatedAtAction(nameof(GetTargets),new {id = newTarget.Id}, newTarget);
    }
}