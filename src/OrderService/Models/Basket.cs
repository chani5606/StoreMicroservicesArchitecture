namespace OrderService.Models;

public sealed class Basket
{
    public int Id { get; set; }

    public Guid CorrelationId { get; set; }

    public int LegacyId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; } = 1;

    public string Status { get; set; } = "Pending";

    public decimal UnitPrice { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}