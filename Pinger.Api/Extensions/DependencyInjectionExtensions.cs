using Microsoft.EntityFrameworkCore;
using Pinger.Application.Services.Interface;
using Pinger.Application.Utility;
using Pinger.Infrastructure.Persistence;
using Pinger.Infrastructure.Services;

namespace Pinger.Api.Extensions;

public static class DependencyInjectionExtensions
{
    private static readonly Type[] ApplicationServices =
    {
        typeof(AuthService),
        typeof(UserService),
        typeof(DeviceHubService),
    };

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure SQLite
        services.AddDbContext<AppDbContext>(options => 
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Sessions persist across multiple HTTP requests, keep one instance
        services.AddSingleton<SignalRSessionManager>();

        foreach (var serviceType in ApplicationServices)
        {
            var interfaceType = serviceType.GetInterfaces()[0];
            services.AddScoped(interfaceType, serviceType);
        }

        return services;
    }
}