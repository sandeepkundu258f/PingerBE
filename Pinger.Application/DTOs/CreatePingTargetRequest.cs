namespace Pinger.Application.DTOs;

public record CreatePingTargetRequest(string Name, string Url, int IntervalSeconds=60);