using System.Security.Claims;
using Pinger.Application.Records;

namespace Pinger.Application.Services.Interface;

public interface IUserService
{
    Task<EndPointResponseRecord<string>> DeactivateUser(int id, ClaimsPrincipal userClaims);
    Task<EndPointResponseRecord<string>> ReactivateUser(int id);
    Task<EndPointResponseRecord<string>> RemoveUser(int id, ClaimsPrincipal userClaims);
}