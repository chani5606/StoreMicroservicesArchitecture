using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.Data;

public sealed class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Winner> Winners => Set<Winner>();

    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Winner>(entity =>
        {
            entity.HasKey(winner => winner.Id);
            entity.Property(winner => winner.ProductId).IsRequired();
            entity.Property(winner => winner.UserId).IsRequired();
            entity.Property(winner => winner.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(winner => winner.UserEmail).HasMaxLength(200).IsRequired();
            entity.Property(winner => winner.WonAtUtc).IsRequired();
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(log => log.Id);
            entity.Property(log => log.RecipientEmail).HasMaxLength(200).IsRequired();
            entity.Property(log => log.Subject).HasMaxLength(300).IsRequired();
            entity.Property(log => log.SentAtUtc).IsRequired();
        });
    }
}