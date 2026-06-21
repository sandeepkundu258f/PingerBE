using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pinger.Application.Domain;
using Pinger.Application.DTOs.UserDTOs;
using Pinger.Application.Enums;
using Pinger.Application.Records;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Infrastructure.Services;

public class UserService(AppDbContext dbcontext) : IUserService
{
    public async Task<EndPointResponseRecord<string>> DeactivateUser(int targetUserid, ClaimsPrincipal userClaims)
    {
        try
        {
            var check = IsAuthorizedForSelfOrHaveRights(targetUserid, userClaims);
            if (check != null)
                return check;

            return await ChangeUserState(targetUserid, true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EndPointResponseRecord<string>> ReactivateUser(int targetUserid)
    {
        try
        {
            return await ChangeUserState(targetUserid, false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EndPointResponseRecord<string>> RemoveUser(int targetUserid, ClaimsPrincipal userClaims)
    {
        try
        {
            var check = IsAuthorizedForSelfOrHaveRights(targetUserid, userClaims);
            if (check != null)
                return check;
        
            var user =  await FindUserById(targetUserid);
        
            if (user == null)
                return new EndPointResponseRecord<string>(StatusCodes.Status404NotFound, "User not found.");
        
            dbcontext.Remove(user);
            await dbcontext.SaveChangesAsync();
            return new EndPointResponseRecord<string>(StatusCodes.Status200OK, $"User {targetUserid} removed");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EndPointResponseRecord<AllUserDetailResponseDto>> FetchAllUserDetails()
    {
        try
        {
            var users = await dbcontext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x=>x.Role)
                .Select(u => new UserDetailResponseDto
                {
                    UserId = u.Id,
                    Username = u.Username,
                    CreatedAt =  u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    IsDeleted =  u.IsDeleted,
                    UserRoles = u.UserRoles
                        .Select(ur => new UserRoleResponseDto
                        {
                            RoleId = ur.RoleId,
                            RoleName = ur.Role!=null ? ur.Role.Name : string.Empty,
                            CreatedAt = ur.CreatedAt,
                            UpdatedAt = ur.UpdatedAt
                        }).ToList()
                })
                .ToListAsync();
            var allUserDetails = new AllUserDetailResponseDto
            {
                UserDetails = users
            };
            return new EndPointResponseRecord<AllUserDetailResponseDto>(StatusCodes.Status200OK, allUserDetails);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EndPointResponseRecord<UserDetailResponseDto>> FetchUserDetails(int targetUserid , ClaimsPrincipal userClaims)
    {
        try
        {
            var check = IsAuthorizedForSelfOrHaveRights(targetUserid, userClaims);
            if (check != null)
                return new EndPointResponseRecord<UserDetailResponseDto>(check.StatusCode, null, check.Message);
            
            var user = await dbcontext.Users
                .Include(x=>x.UserRoles)
                .ThenInclude(y=>y.Role)
                .Where(x=>x.Id == targetUserid)
                .Select(u => new UserDetailResponseDto
                {
                    UserId = u.Id,
                    Username = u.Username,
                    CreatedAt =  u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    IsDeleted =  u.IsDeleted,
                    UserRoles = u.UserRoles
                        .Select(ur => new UserRoleResponseDto
                        {
                            RoleId = ur.RoleId,
                            RoleName = ur.Role!=null ? ur.Role.Name : string.Empty,
                            CreatedAt = ur.CreatedAt,
                            UpdatedAt = ur.UpdatedAt
                        }).ToList()
                })
                .FirstOrDefaultAsync();
            
            if (user is null)
                return new EndPointResponseRecord<UserDetailResponseDto>(StatusCodes.Status404NotFound, null,"User not found.");
            
            return new EndPointResponseRecord<UserDetailResponseDto>(StatusCodes.Status200OK, user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<EndPointResponseRecord<string>> ChangeUserState(int id, bool isDeleted)
    {
        try
        {
            var user =  await FindUserById(id);

            if (user == null)
                return new EndPointResponseRecord<string>(StatusCodes.Status404NotFound,null, "User not found.");
        
            if (user.IsDeleted == isDeleted)
                return new EndPointResponseRecord<string>(StatusCodes.Status200OK,null, $"User {id} already {(isDeleted? "inactive": "active")}");
        
            user.IsDeleted = isDeleted;
            foreach (var userRole in user.UserRoles)
            {
                userRole.IsDeleted = isDeleted;
            }
        
            await dbcontext.SaveChangesAsync();
            return new EndPointResponseRecord<string>(StatusCodes.Status200OK,null, $"User {id} is {(isDeleted? "deactivated": "reactivated")}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    private async Task<User?> FindUserById(int id)
    {
        try
        {
            return await dbcontext.Users
                .Include(x=>x.UserRoles)
                .FirstOrDefaultAsync(x=>x.Id == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    private EndPointResponseRecord<string>? IsAuthorizedForSelfOrHaveRights(int targetUserId, ClaimsPrincipal userClaims)
    {
        try
        {
            var loggedInUserId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(loggedInUserId))
                return new EndPointResponseRecord<string>(StatusCodes.Status401Unauthorized,null,"Could not identify the logged in user.");
        
            var loggedInUserIsAdmin = userClaims.IsInRole(nameof(RoleEnum.Admin))||userClaims.IsInRole(nameof(RoleEnum.SuperAdmin));

            if (!loggedInUserIsAdmin && loggedInUserId != targetUserId.ToString())
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