using Microsoft.AspNetCore.Mvc;
using Pinger.Application.DTOs;
using Pinger.Application.Services.Interface;

namespace Pinger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto requestDto)
    {
        try
        {
            var success = await authService.RegisterAsync(requestDto);
            if (!success)
                return Conflict("Username already taken");
        
            return Ok("User registered successfully");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginUser([FromForm] LoginRequestDto requestDto)
    {
        try
        {
            var token = await authService.LoginAsync(requestDto);
            if (token == null)
                return Unauthorized("Invalid creds");

            return Ok(
                new
                {
                    access_token = token ,
                    token_type = "Bearer"
                });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }
}