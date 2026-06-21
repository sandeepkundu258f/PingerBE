namespace Pinger.Application.DTOs.AuthDTOs;

public record RegisterRequestDto(string Username, string Password, List<int>? RoleIds = null);