namespace Pinger.Application.DTOs;

public record CreatePingTargetRequestDto(string Name, string Url, int IntervalSeconds=60);