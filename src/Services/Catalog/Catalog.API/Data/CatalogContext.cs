using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

public class CatalogContext : ICatalogContext
{
    private IMongoDatabase _db { get; set; }
    private MongoClient _mongoClient { get; set; }
    public IClientSessionHandle Session { get; set; }

    public CatalogContext(IConfiguration configuration)
    {
        _mongoClient = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        _db =_mongoClient.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
        
        //Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
        CatalogContextSeed.SeedData(GetCollection<Product>("Products"));
    }
    //public IMongoCollection<Product> Products {get;set;}
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _db.GetCollection<T>(name);
    }
}