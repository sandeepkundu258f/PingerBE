namespace Pinger.Application.Domain;

public class UserSession : BaseEntity
{
    public Guid SessionId { get; init; } = Guid.NewGuid();
    public int UserId { get; init; }
    public User? User { get; init; }
    
    public required string DeviceName { get; set; }
    public required Guid DeviceIdentifier { get; init; }
    public required string IpAddress { get; set; }

    public bool IsOnline { get; set; } = false;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    
    // isDeleted in ignored in DbContext
}