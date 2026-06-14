using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;
using Pinger.Infrastructure.Services;
using Scalar.AspNetCore;
//using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPingTargetService, PingTargetService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
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
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

/*
// Add security scheme to Scalar UI configurations so it gives us an "Authorize" lock icon
builder.Services.AddOpenApi(options =>
{
    // Discarded 'context' and 'cancellationToken' with '_' to clear the unused parameter warnings
    options.AddDocumentTransformer((document, _, _) => 
    {
        document.Components ??= new OpenApiComponents();
        
        // 1. Use IOpenApiSecurityScheme interface to satisfy the dictionary constraint
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Input your JWT token directly into the field below."
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();
        
        // 2. Use the new OpenApiSecuritySchemeReference class directly as the dictionary key
        document.Security.Add(new OpenApiSecurityRequirement
        {
            // Fix: Replaced Array.Empty<string>() with new List<string>()
            [new OpenApiSecuritySchemeReference("Bearer")] = []
        });
        
        return Task.CompletedTask;
    });
});

*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();