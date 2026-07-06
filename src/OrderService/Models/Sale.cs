namespace OrderService.Models;

public sealed class Sale
{
    public int Id { get; set; }

    public int LegacyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime StartDateUtc { get; set; }

    public DateTime EndDateUtc { get; set; }
}