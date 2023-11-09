using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ICatalogContext _context;

    public ProductRepository(ICatalogContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task CreateProduct(Product product)
    {
        await _context.GetCollection<Product>("Products").InsertOneAsync(product);
    }

    public async Task<bool> DeleteProduct(string id)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        DeleteResult deleteResult = await _context.GetCollection<Product>("Products").DeleteOneAsync(filter);

        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }

    public async Task<Product> GetProduct(string id)
    {
        return await _context.GetCollection<Product>("Products").Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
        return await _context.GetCollection<Product>("Products").Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByName(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Name, name);
        return await _context.GetCollection<Product>("Products").Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _context.GetCollection<Product>("Products").Find(p => true).ToListAsync();
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        var updateResult = await _context.GetCollection<Product>("Products")
                .ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);

        return updateResult.IsAcknowledged
                && updateResult.ModifiedCount > 0;
    }
}