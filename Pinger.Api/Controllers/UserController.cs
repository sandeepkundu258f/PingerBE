using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinger.Application.Enums;
using Pinger.Application.Services.Interface;

namespace Pinger.Api.Controllers;

[ApiController]
[Route("api/[controller]/{id:int}")]
[Authorize]
public class UserController(IUserService userService): ControllerBase
{
    [HttpPatch("Deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        try
        {
            var result = await userService.DeactivateUser(id, User);
            return StatusCode(result.StatusCode, new {message = result.Payload});
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [Authorize(Roles = $"{nameof(RoleEnum.Admin)}")]
    [HttpPatch("Reactivate")]
    public async Task<IActionResult> ReactivateUser(int id)
    {
        try
        {
            var result = await userService.ReactivateUser(id);
            return StatusCode(result.StatusCode, new {message = result.Payload});
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }
    
    [HttpDelete("Remove")]
    public async Task<IActionResult> RemoveUser(int id)
    {
        try
        {
            var result = await userService.RemoveUser(id, User);
            return StatusCode(result.StatusCode, new {message = result.Payload});
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}