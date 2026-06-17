using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinger.Application.DTOs;
using Pinger.Application.Enums;
using Pinger.Application.Services.Interface;

namespace Pinger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TargetsController(IPingTargetService pingTargetService) : ControllerBase
{
    [Authorize(Roles = $"{nameof(RoleEnum.Admin)}")]
    [HttpGet("getAllTargets")]
    public async Task<IActionResult> GetTargets()
    {
        var targets = await pingTargetService.GetAllPingTargetsAsync();
        return Ok(targets);
    }

    [HttpPost("createPingTarget")]
    public async Task<IActionResult> CreateTarget([FromBody] CreatePingTargetRequestDto requestDto)
    {
        if (string.IsNullOrWhiteSpace(requestDto.Name) || string.IsNullOrWhiteSpace(requestDto.Url))
        {
            return BadRequest("Name and URL are required.");
        }
        
        var newTarget = await pingTargetService.CreatePingTargetAsync(
            requestDto.Name,
            requestDto.Url,
            requestDto.IntervalSeconds
        );
        
        return CreatedAtAction(nameof(GetTargets),new {id = newTarget.Id}, newTarget);
    }
}