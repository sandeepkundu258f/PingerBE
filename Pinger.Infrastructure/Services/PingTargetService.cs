using Microsoft.EntityFrameworkCore;
using Pinger.Application.Domain;
using Pinger.Application.Services;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Infrastructure.Services;

public class PingTargetService(AppDbContext dbContext) : IPingTargetService
{
    public async Task<IEnumerable<PingTarget>> GetAllPingTargetsAsync()
    {
        return await dbContext.PingTargets.ToListAsync();
    }

    public async Task<PingTarget> CreatePingTargetAsync(string name, string url, int intervalSeconds = 60)
    {
        var target = new PingTarget
        {
            Name = name,
            Url = url,
            IntervalSeconds =  intervalSeconds
        };
        
        await dbContext.PingTargets.AddAsync(target);
        await dbContext.SaveChangesAsync();
        return target;
    }
}