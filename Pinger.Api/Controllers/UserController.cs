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
        var result = await userService.DeactivateUser(id, User);
        return result switch
        {
            HttpStatusEnum.Unauthorized => Unauthorized("Could not identify the logged in user."),
            HttpStatusEnum.Forbidden => StatusCode(
                StatusCodes.Status403Forbidden, 
                new 
                { 
                    message = "You are not authorized to deactivate this user's account." 
                }
            ),
            HttpStatusEnum.Ok => Ok(new { message = $"User {id} was successfully deactivated." }),
            HttpStatusEnum.NotFound => NotFound("Target user does not exist."),
            _ => BadRequest()
        };
    }
    
    [Authorize(Roles = $"{nameof(RoleEnum.Admin)}")]
    [HttpPatch("Reactivate")]
    public async Task<IActionResult> ReactivateUser(int id)
    {
        var result = await userService.ReactivateUser(id);
        return result switch
        {
            HttpStatusEnum.Ok => Ok(new { message = $"User {id} was successfully reactivated." }),
            HttpStatusEnum.NotFound => NotFound("Target user does not exist."),
            _ => BadRequest()
        };
    }
}