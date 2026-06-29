using Pinger.Application.Enums;
using Pinger.Application.Records;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Security.Claims;
using Pinger.Application.Domain;

namespace Pinger.Application.Utility;

public class RightsUtility
{
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
    
    public static EndPointResponseRecord<string>? PreventSuperAdminModification(User? targetUser, ClaimsPrincipal userClaims)
    {
        try
        {
            if (targetUser == null)
                return new EndPointResponseRecord<string>(StatusCodes.Status404NotFound,null,"Could not identify the target user.");
            
            var loggedInUserId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(loggedInUserId))
                return new EndPointResponseRecord<string>(StatusCodes.Status401Unauthorized,null,"Could not identify the logged in user.");

            var isTargetUserSuperAdmin = targetUser.UserRoles.Any(x => x.RoleId == (int)RoleEnum.SuperAdmin);
            var isLoggedInUserSuperAdmin = userClaims.IsInRole(nameof(RoleEnum.SuperAdmin));

            if (isTargetUserSuperAdmin && !isLoggedInUserSuperAdmin)
            {
                return new EndPointResponseRecord<string>(StatusCodes.Status403Forbidden,null,"You are not authorized to perform this action.");
            }
            
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}