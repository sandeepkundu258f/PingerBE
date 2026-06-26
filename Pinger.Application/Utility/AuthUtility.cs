using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pinger.Application.Domain;
using Pinger.Application.Enums;
using Pinger.Application.Records;

namespace Pinger.Application.Utility;

public static class AuthUtility
{
    public static string GenerateJwtToken(User user, IConfigurationSection jwtSettings)
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key missing"));

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        ];

        foreach (var userRole in user.UserRoles)
        {
            if (userRole.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public static EndPointResponseRecord<string>? IsAuthorizedForSelfOrHaveRights(int targetUserId, ClaimsPrincipal userClaims, List<RoleEnum> userRolesAllowed)
    {
        try
        {
            var loggedInUserId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(loggedInUserId))
                return new EndPointResponseRecord<string>(StatusCodes.Status401Unauthorized,null,"Could not identify the logged in user.");
            
            var loggedInUserIsAllowed = userRolesAllowed.Any(x =>
            {
                if (!Enum.IsDefined(typeof(RoleEnum), x))
                    throw new InvalidEnumArgumentException(nameof(x), (int)x, typeof(RoleEnum));
                return userClaims.IsInRole(x.ToString());
            });

            if (!loggedInUserIsAllowed && loggedInUserId != targetUserId.ToString())
                return new EndPointResponseRecord<string>(StatusCodes.Status403Forbidden,null,"You are not authorized to perform this action.");

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}