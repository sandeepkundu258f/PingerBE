namespace Pinger.Application.Domain;

public class UserSession : BaseEntity
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public required string DeviceName { get; set; }
    public required string DeviceIdentifier { get; set; }
    public required string IpAddress { get; set; }
    
    public bool IsOnline { get; set; }
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    
    // isDeleted in ignored in DbContext
}