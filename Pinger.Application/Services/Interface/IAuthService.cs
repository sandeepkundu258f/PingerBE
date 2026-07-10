using Microsoft.AspNetCore.Http;
using Pinger.Application.DTOs.AuthDTOs;

namespace Pinger.Application.Services.Interface;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequestDto registerRequest);
    Task<string?> LoginAsync(LoginRequestDto loginRequest, HttpRequest httpRequest);
}