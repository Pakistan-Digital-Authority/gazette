using gop.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace gop.Data;

/// <summary>
/// Database context for the auth service
/// </summary>
public class DatabaseContext : DbContext
{
    /// <summary>
    /// Auth Context ctr
    /// </summary>
    /// <param name="options"></param>
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    /// <summary>
    /// On model creating
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasIndex(l => l.CreatedAt);
            entity.HasIndex(l => l.Level);
            entity.HasIndex(l => l.UserId);
            entity.HasIndex(l => l.StatusCode);
            
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Notice)
                .WithMany(nc => nc.Notifications)
                .HasForeignKey(n => n.NoticeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Token> Tokens { get; set; }
    
    public DbSet<LoginHistory> LoginHistories { get; set; }
    
    public DbSet<Notice> Notices { get; set; }
    
    public DbSet<ActReference> ActReferences { get; set; }
    
    public DbSet<NoticeActReference> NoticeActReferences { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<SroCounter> SroCounters { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    
}