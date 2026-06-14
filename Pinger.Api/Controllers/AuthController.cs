using Microsoft.AspNetCore.Mvc;
using Pinger.Application.DTOs;
using Pinger.Application.Services.Interface;

namespace Pinger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto requestDto)
    {
        var success = await authService.RegisterAsync(requestDto);
        if (!success)
            return BadRequest("Username already taken");
        
        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto requestDto)
    {
        var token = await authService.LoginAsync(requestDto);
        if (token == null)
            return Unauthorized("Invalid creds");

        return Ok(new { Token = token });
    }
}