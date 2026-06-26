using System.Security.Claims;
using Pinger.Application.DTOs.UserDTOs;
using Pinger.Application.Records;

namespace Pinger.Application.Services.Interface;

public interface IUserService
{
    Task<EndPointResponseRecord<string>> DeactivateUser(int targetUserid, ClaimsPrincipal userClaims);
    Task<EndPointResponseRecord<string>> ReactivateUser(int targetUserid);
    Task<EndPointResponseRecord<string>> RemoveUser(int targetUserid, ClaimsPrincipal userClaims);
    Task<EndPointResponseRecord<AllUserDetailResponseDto>> FetchAllUserDetails();
    Task<EndPointResponseRecord<UserDetailResponseDto>> FetchUserDetails(int targetUserid, ClaimsPrincipal userClaims);
}