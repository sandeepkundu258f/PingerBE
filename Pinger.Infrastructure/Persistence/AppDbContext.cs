using Microsoft.EntityFrameworkCore;
using Pinger.Application.Domain;

namespace Pinger.Infrastructure.Persistence;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<PingTarget>  PingTargets { get; set; }
    public DbSet<PingLog> PingLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PingTarget>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x=>x.Name).IsUnique();
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(x=>x.IntervalSeconds)
                .IsRequired();
            entity.Property(x=>x.IsActive)
                .IsRequired();
            entity.Property(x=>x.CreatedAt)
                .IsRequired();
            entity.Property(x => x.UpdatedAt)
                .IsRequired(false);
        });

        modelBuilder.Entity<PingLog>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.PingTarget)
                .WithMany(y => y.PingLogs)
                .HasForeignKey(x => x.PingTargetId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}