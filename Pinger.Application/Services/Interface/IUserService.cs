using System.Security.Claims;
using Pinger.Application.Enums;

namespace Pinger.Application.Services.Interface;

public interface IUserService
{
    Task<HttpStatusEnum> DeactivateUser(int id, ClaimsPrincipal userClaims);
    Task<HttpStatusEnum> ReactivateUser(int id);
}