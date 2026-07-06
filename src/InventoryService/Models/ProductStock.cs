namespace InventoryService.Models;

public sealed class ProductStock
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int DonorId { get; set; }

    public int QuantityAvailable { get; set; }

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Donor? Donor { get; set; }
}