using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinger.Application.DTOs;
using Pinger.Application.DTOs.UserDTOs;
using Pinger.Application.Enums;
using Pinger.Application.Services.Interface;

namespace Pinger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService): ControllerBase
{
    [HttpPatch("{id:int}/Deactivate")]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        try
        {
            var result = await userService.DeactivateUser(id, User);
            return StatusCode(result.StatusCode, new StandardResponseDto(result.Message));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [Authorize(Roles = $"{nameof(RoleEnum.Admin)},{nameof(RoleEnum.SuperAdmin)}")]
    [HttpPatch("{id:int}/Reactivate")]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReactivateUser(int id)
    {
        try
        {
            var result = await userService.ReactivateUser(id);
            return StatusCode(result.StatusCode, new StandardResponseDto(result.Message));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }
    
    [HttpDelete("{id:int}/Remove")]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveUser(int id)
    {
        try
        {
            var result = await userService.RemoveUser(id, User);
            return StatusCode(result.StatusCode, new StandardResponseDto(result.Message));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = $"{nameof(RoleEnum.Admin)},{nameof(RoleEnum.SuperAdmin)}")]
    [HttpGet("FetchAll")]
    [ProducesResponseType(typeof(AllUserDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> FetchAllUserDetails()
    {
        try
        {
            var result = await userService.FetchAllUserDetails();
            return StatusCode(result.StatusCode, result.Payload);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("{id:int}/Fetch")]
    [ProducesResponseType(typeof(AllUserDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(StandardResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FetchUserDetail(int id)
    {
        try
        {
            var result = await userService.FetchUserDetails(id, User);
            if(result.StatusCode 
               is StatusCodes.Status401Unauthorized 
               or StatusCodes.Status403Forbidden 
               or StatusCodes.Status404NotFound)
                return StatusCode(result.StatusCode, result.Message);
            
            return StatusCode(result.StatusCode, result.Payload);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}