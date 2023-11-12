using Discount.Grpc.Entities;
using Discount.Grpc.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Discount.IntegrationTest;

public class DiscountRepositoryTest
{

    [Fact]
    public async Task Create_Discount_Test()
    {
        //Arrange
        IConfigurationRoot configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

        DiscountRepository repository = new DiscountRepository(configuration);

        Coupon coupon = new Coupon()
        {
            ProductName = "ProductX",
            Description = "DescriptionX",
            Amount = 100
        };

        //-------------INSERT

        //Act
        var isInserted = await repository.CreateDiscount(coupon);
        var insertedCoupon = await repository.GetDiscount("ProductX");

        //Assert
        isInserted.Should().BeTrue();

        insertedCoupon.Should().NotBeNull();

        //------------UPDATE
        insertedCoupon.ProductName += "Updated";

        //Act
        var isUpdated = await repository.UpdateDiscount(insertedCoupon);
        var updatedCoupon = await repository.GetDiscount(insertedCoupon.ProductName);

        //Assert
        isUpdated.Should().BeTrue();

        updatedCoupon.Should().NotBeNull();

        //-------------DELETE
        //Act
        var isDeleted = await repository.DeleteDiscount(updatedCoupon.ProductName);
        var deletedCoupon = await repository.GetDiscount(updatedCoupon.ProductName);

        //Assert
        isUpdated.Should().BeTrue();

        deletedCoupon.ProductName.Should().BeSameAs("No Discount");

    }
}