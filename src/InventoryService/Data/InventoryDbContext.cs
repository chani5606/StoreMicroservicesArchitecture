using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Data;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Donor> Donors => Set<Donor>();

    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(donor => donor.Id);
            entity.Property(donor => donor.Name).HasMaxLength(100).IsRequired();
            entity.Property(donor => donor.Email).HasMaxLength(150).IsRequired();
            entity.Property(donor => donor.Phone).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<ProductStock>(entity =>
        {
            entity.HasKey(stock => stock.Id);
            entity.Property(stock => stock.ProductId).IsRequired();
            entity.Property(stock => stock.QuantityAvailable).IsRequired();
            entity.Property(stock => stock.UpdatedAtUtc).IsRequired();

            entity.HasOne(stock => stock.Donor)
                .WithMany(donor => donor.ProductStocks)
                .HasForeignKey(stock => stock.DonorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}