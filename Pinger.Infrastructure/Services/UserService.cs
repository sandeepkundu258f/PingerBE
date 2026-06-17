using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Pinger.Application.Enums;
using Pinger.Application.Services.Interface;
using Pinger.Infrastructure.Persistence;

namespace Pinger.Infrastructure.Services;

public class UserService(AppDbContext dbcontext) : IUserService
{
    public async Task<HttpStatusEnum> DeactivateUser(int id, ClaimsPrincipal userClaims)
    {
        var loggedInUserId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(loggedInUserId))
            return HttpStatusEnum.Unauthorized;
        
        var loggedInUserIsAdmin = userClaims.IsInRole(nameof(RoleEnum.Admin));

        if (!loggedInUserIsAdmin && loggedInUserId != id.ToString())
            return HttpStatusEnum.Forbidden;

        return await ChangeUserState(id, true);
    }

    public async Task<HttpStatusEnum> ReactivateUser(int id)
    {
        return await ChangeUserState(id, false);
    }

    private async Task<HttpStatusEnum> ChangeUserState(int id, bool isDeleted)
    {
        var user =  await dbcontext.Users
            .Include(x=>x.UserRoles)
            .Where(x=>x.Id == id)
            .FirstOrDefaultAsync();

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
}