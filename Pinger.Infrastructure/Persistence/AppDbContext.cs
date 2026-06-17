using Microsoft.EntityFrameworkCore;
using Pinger.Application.Domain;
using Pinger.Application.Enums;

namespace Pinger.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PingTarget>  PingTargets { get; set; }
    public DbSet<PingLog> PingLogs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
    
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
        });

        modelBuilder.Entity<PingLog>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.PingTarget)
                .WithMany(y => y.PingLogs)
                .HasForeignKey(x => x.PingTargetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(x=>x.Id);
            entity.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();
            
            entity.HasOne(x => x.User)
                .WithMany(y  => y.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(x => x.Role)
                .WithMany(y => y.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(r => r.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = (int)RoleEnum.Admin, Name = nameof(RoleEnum.Admin), IsDeleted = false },
            new Role { Id = (int)RoleEnum.User, Name =  nameof(RoleEnum.User), IsDeleted = false }
        );
    }
}