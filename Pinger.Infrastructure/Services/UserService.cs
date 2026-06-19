using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pinger.Application.Domain;
using Pinger.Application.Enums;
using Pinger.Application.Records;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Infrastructure.Services;

public class UserService(AppDbContext dbcontext) : IUserService
{
    public async Task<EndPointResponseRecord<string>> DeactivateUser(int id, ClaimsPrincipal userClaims)
    {
        try
        {
            var check = CheckUserRight(id, userClaims);
            if (check != null)
                return check;

            return await ChangeUserState(id, true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EndPointResponseRecord<string>> ReactivateUser(int id)
    {
        try
        {
            return await ChangeUserState(id, false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EndPointResponseRecord<string>> RemoveUser(int id, ClaimsPrincipal userClaims)
    {
        try
        {
            var check = CheckUserRight(id, userClaims);
            if (check != null)
                return check;
        
            var user =  await FindUserById(id);
        
            if (user == null)
                return new EndPointResponseRecord<string>(StatusCodes.Status404NotFound, "User not found.");
        
            dbcontext.Remove(user);
            await dbcontext.SaveChangesAsync();
            return new EndPointResponseRecord<string>(StatusCodes.Status200OK, $"User {id} removed");
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
                return new EndPointResponseRecord<string>(StatusCodes.Status404NotFound, "User not found.");
        
            if (user.IsDeleted == isDeleted)
                return new EndPointResponseRecord<string>(StatusCodes.Status200OK, $"User {id} already {(isDeleted? "inactive": "active")}");
        
            user.IsDeleted = isDeleted;
            foreach (var userRole in user.UserRoles)
            {
                userRole.IsDeleted = isDeleted;
            }
        
            await dbcontext.SaveChangesAsync();
            return new EndPointResponseRecord<string>(StatusCodes.Status200OK, $"User {id} is {(isDeleted? "deactivated": "reactivated")}");
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

    private EndPointResponseRecord<string>? CheckUserRight(int id, ClaimsPrincipal userClaims)
    {
        try
        {
            var loggedInUserId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(loggedInUserId))
                return new EndPointResponseRecord<string>(StatusCodes.Status401Unauthorized,"Could not identify the logged in user.");
        
            var loggedInUserIsAdmin = userClaims.IsInRole(nameof(RoleEnum.Admin));

            if (!loggedInUserIsAdmin && loggedInUserId != id.ToString())
                return new EndPointResponseRecord<string>(StatusCodes.Status403Forbidden,"You are not authorized to perform this action on this user's account.");

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}