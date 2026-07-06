namespace ProductCatalogService.Data;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public string ProductsCollectionName { get; set; } = "products";

    public string CategoriesCollectionName { get; set; } = "categories";
}