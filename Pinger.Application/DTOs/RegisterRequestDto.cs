namespace Pinger.Application.DTOs;

public record RegisterRequestDto(string Username, string Password, List<int>? RoleIds = null);