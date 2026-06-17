namespace Pinger.Application.Domain;

public class PingLog : BaseEntity
{
    public int PingTargetId { get; set; }
    public int ResponseTimeMs { get; set; }
    public required bool IsSuccess { get; set; }
    
    public PingTarget? PingTarget { get; set; }
}