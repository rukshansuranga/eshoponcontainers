using System.Diagnostics.CodeAnalysis;
using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Catalog.Testing;

public class CatalogRepository
{
    [Fact]
    public async void CatalogRepositoryCreateTest()
    {
        //Arrange
        var mockContext = new Mock<ICatalogContext>();
    
        var settings = new MongoCollectionSettings();
        var mockCollection = new Mock<IMongoCollection<Product>> { DefaultValue = DefaultValue.Mock };
        mockCollection.SetupGet(s => s.DocumentSerializer).Returns(settings.SerializerRegistry.GetSerializer<Product>());
        mockCollection.SetupGet(s => s.Settings).Returns(settings);

        var collection =  mockCollection.Object;

        var document = new Product()
                                {
                                    Id = "602d2149e773f2a3990b47f5",
                                    Name = "IPhone X",
                                    Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                                    ImageFile = "product-1.png",
                                    Price = 950.00M,
                                    Category = "Smart Phone"
                                };

        collection.InsertOne(document);

        mockContext.Setup(x => x.GetCollection<Product>("Products")).Returns(collection);
        
        //act
        var sut = new ProductRepository(mockContext.Object);

        await sut.CreateProduct(document);

        var test1 = mockCollection.Object.Find(p => true).ToList().Count;
        //var test = mockContext.Object.GetCollection<Product>("Products").Find(x => x.Id == id).FirstOrDefault();

        Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"+ test1);

        //asert
        //mockCatalogContext.Object.Products.Find(x => x.Id == id).ToList().Count.Equals(1);
        //mockCatalogContext.Object.Products.Find(x => x.Id == id).FirstOrDefault().Name.Equals("xx");
    }
}