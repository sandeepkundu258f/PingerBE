using Microsoft.EntityFrameworkCore;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;
using Pinger.Infrastructure.Services;

namespace Pinger.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure SQLite
        services.AddDbContext<AppDbContext>(options => 
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Register Application/Infrastructure dependencies
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}