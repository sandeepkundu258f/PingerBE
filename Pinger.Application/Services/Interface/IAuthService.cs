using Pinger.Application.DTOs;

namespace Pinger.Application.Services.Interface;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequestDto registerRequest);
    Task<string?> LoginAsync(LoginRequestDto loginRequest);
}