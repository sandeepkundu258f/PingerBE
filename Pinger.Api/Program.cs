using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;
using Pinger.Infrastructure.Services;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPingTargetService, PingTargetService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

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
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                // Get your Database Context from the request's DI container
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                //Extract the User ID from the token's claims
                var userIdStr = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                {
                    context.Fail("Unauthorized: Token contains invalid user data.");
                    return;
                }

                //Query the DB to check if the user still exists and is active
                var userExistsAndActive = await dbContext.Users
                    .AnyAsync(u => u.Id == userId && u.IsDeleted == false);

                if (!userExistsAndActive)
                {
                    // Instantly neutralizes the token and forces a 401 Unauthorized response
                    context.Fail("Unauthorized: This account has been deleted or deactivated.");
                }
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

//Register Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pinger API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();