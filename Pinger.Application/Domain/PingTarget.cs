namespace Pinger.Application.Domain;

public class PingTarget
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public int IntervalSeconds { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<PingLog> PingLogs { get; set; } = new List<PingLog>();
}