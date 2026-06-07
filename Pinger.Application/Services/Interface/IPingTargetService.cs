using Pinger.Application.Domain;

namespace Pinger.Application.Services.Interface;

public interface IPingTargetService
{
    Task<IEnumerable<PingTarget>> GetAllPingTargetsAsync();
    Task<PingTarget> CreatePingTargetAsync(string name, string url, int intervalSeconds = 60);
}