namespace Pinger.Application.Domain;

public class PingLog
{
    public int Id { get; set; }
    public int PingTargetId { get; set; }
    public int ResponseTimeMs { get; set; }
    public required bool IsSuccess { get; set; }
    public required DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public PingTarget? PingTarget { get; set; }
}