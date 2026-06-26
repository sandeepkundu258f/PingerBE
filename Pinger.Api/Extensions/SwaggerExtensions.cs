using Microsoft.OpenApi;

namespace Pinger.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pinger API", Version = "v1" });

            //Define OAuth2 Password Flow to generate Username/Password fields
            options.AddSecurityDefinition("OAuth2Password", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Enter your username and password to log in and automatically fetch your JWT.",
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        // Points directly to your login endpoint
                        TokenUrl = new Uri("/api/Auth/login", UriKind.Relative) 
                    }
                }
            });

            //Apply this requirement globally
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("OAuth2Password", document)] = []
            });
        });

        return services;
    }
}