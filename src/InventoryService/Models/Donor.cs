namespace InventoryService.Models;

public sealed class Donor
{
    public int Id { get; set; }

    public int LegacyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Neighborhood { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
}