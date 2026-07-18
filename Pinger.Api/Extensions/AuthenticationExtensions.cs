using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        // Get your Database Context from the request's DI container
                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                        //Extract the User ID from the token's claims
                        var userIdStr = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var sessionIdStr = context.Principal?.FindFirst("SessionId")?.Value;

                        if (string.IsNullOrEmpty(userIdStr) 
                            || !int.TryParse(userIdStr, out var userId)
                            ||string.IsNullOrEmpty(sessionIdStr) 
                            || !Guid.TryParse(sessionIdStr, out var sessionIdGuid))
                        {
                            context.Fail("Unauthorized: Token contains invalid user data.");
                            return;
                        }

                        //Query the DB to check if the user still exists and is active
                        var userExistsAndActive = await dbContext.Users
                            .AnyAsync(u => u.Id == userId && u.IsDeleted == false);
                        
                        var userSessionExists = await dbContext.UserSessions
                            .AnyAsync(u => u.SessionId ==  sessionIdGuid && u.UserId == userId);

                        if (!userExistsAndActive || !userSessionExists)
                        {
                            // Instantly neutralizes the token and forces a 401 Unauthorized response
                            context.Fail("Unauthorized: This account has been deleted or deactivated.");
                        }
                    }
                };
            });

        return services;
    }
}