namespace Pinger.Application.Domain;

public class PingTarget: BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public int IntervalSeconds { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    
    public ICollection<PingLog> PingLogs { get; set; } = new List<PingLog>();
}