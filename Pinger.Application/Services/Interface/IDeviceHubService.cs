using System.Security.Claims;
using Pinger.Application.Domain;
using Pinger.Application.DTOs.DeviceHubDTOs;

namespace Pinger.Application.Services.Interface;

public interface IDeviceHubService
{
     Task<UserSession?> ActivateSession(string sessionId);
     Task<UserSession?> DeactivateSession(string sessionId);
     Task<IEnumerable<DeviceListResponseDto>> ListDevices(int targetUserid);
}