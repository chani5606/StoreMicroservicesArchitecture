using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductCatalogService.Models;

namespace ProductCatalogService.Data;

public sealed class CatalogMongoContext
{
    public CatalogMongoContext(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;
        var client = new MongoClient(mongoSettings.ConnectionString);
        var database = client.GetDatabase(mongoSettings.DatabaseName);

        Products = database.GetCollection<ProductDocument>(mongoSettings.ProductsCollectionName);
        Categories = database.GetCollection<CategoryDocument>(mongoSettings.CategoriesCollectionName);
    }

    public IMongoCollection<ProductDocument> Products { get; }

    public IMongoCollection<CategoryDocument> Categories { get; }
}