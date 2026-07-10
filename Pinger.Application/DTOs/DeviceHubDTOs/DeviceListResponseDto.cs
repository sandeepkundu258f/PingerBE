namespace Pinger.Application.DTOs.DeviceHubDTOs;

public class DeviceListResponseDto
{
    public Guid SessionId { get; set; }
    public required string DeviceName { get; set; }
    public Guid DeviceIdentifier { get; init; }
    public required string IpAddress { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastActiveAt { get; set; }
}