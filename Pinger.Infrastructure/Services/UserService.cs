using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Pinger.Application.Domain;
using Pinger.Application.Enums;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Infrastructure.Services;

public class UserService(AppDbContext dbcontext) : IUserService
{
    public async Task<HttpStatusEnum> DeactivateUser(int id, ClaimsPrincipal userClaims)
    {
        try
        {
            var check = CheckUserRight(id, userClaims);
            if (check != null)
                return check.Value;

            return await ChangeUserState(id, true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<HttpStatusEnum> ReactivateUser(int id)
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

    public async Task<HttpStatusEnum> RemoveUser(int id, ClaimsPrincipal userClaims)
    {
        try
        {
            var check = CheckUserRight(id, userClaims);
            if (check != null)
                return check.Value;
        
            var user =  await FindUserById(id);
        
            if (user == null)
                return HttpStatusEnum.NotFound;
        
            dbcontext.Remove(user);
            await dbcontext.SaveChangesAsync();
            return HttpStatusEnum.Ok;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<HttpStatusEnum> ChangeUserState(int id, bool isDeleted)
    {
        try
        {
            var user =  await FindUserById(id);

            if (user == null)
                return HttpStatusEnum.NotFound;
        
            if (user.IsDeleted == isDeleted)
                return HttpStatusEnum.Ok;
        
            user.IsDeleted = isDeleted;
            foreach (var userRole in user.UserRoles)
            {
                userRole.IsDeleted = isDeleted;
            }
        
            await dbcontext.SaveChangesAsync();
            return HttpStatusEnum.Ok;
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

    private HttpStatusEnum? CheckUserRight(int id, ClaimsPrincipal userClaims)
    {
        try
        {
            var loggedInUserId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(loggedInUserId))
                return HttpStatusEnum.Unauthorized;
        
            var loggedInUserIsAdmin = userClaims.IsInRole(nameof(RoleEnum.Admin));

            if (!loggedInUserIsAdmin && loggedInUserId != id.ToString())
                return HttpStatusEnum.Forbidden;

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}