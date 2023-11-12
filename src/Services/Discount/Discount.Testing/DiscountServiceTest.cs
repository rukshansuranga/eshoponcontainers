using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Mapper;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using FluentAssertions;
using Microsoft.VisualBasic;
using Moq;

namespace Discount.Testing;

public class DiscountServiceTest
{
    IMapper _mapper;
    MapperConfiguration _config;

    Mock<IDiscountRepository> _repository = new Mock<IDiscountRepository>();

    public DiscountServiceTest()
    {
        //_config = new MapperConfiguration(cfg => cfg.AddMaps(new[] {"Discount.Grpc"}));
        _config = new MapperConfiguration(cfg => cfg.AddProfile<DiscountProfile>());

        _mapper = _config.CreateMapper();

    }

    [Fact]
    public void ConfigurationTest()
    {
        _config.AssertConfigurationIsValid();

        Assert.True(true, "message");
    }

    [Fact]
    public async Task Create_Discount_Mapping_Test()
    {
        //Arrange
        CouponModel sourceCouponModel = new()
        {
            Id = 12,
            ProductName = "SampleProduct",
            Description = "TestDescription",
            Amount = 1000
        };

        CreateDiscountRequest sourceCreateDiscountRequest = new()
        {
            Coupon = sourceCouponModel,
        };

        _repository.Setup(x => x.CreateDiscount(It.IsAny<Coupon>())).ReturnsAsync(true);

        //Act
         var destinationCoupon = _mapper.Map<Coupon>(sourceCreateDiscountRequest.Coupon);

        //Assert
        destinationCoupon.ProductName.Should().BeSameAs(sourceCouponModel.ProductName);
        destinationCoupon.Amount.Should().Be(sourceCouponModel.Amount);

         await _repository.Object.CreateDiscount(It.IsAny<Coupon>());

        _repository.Verify(x => x.CreateDiscount(It.IsAny<Coupon>()), Times.Once);

        var couponModel = _mapper.Map<CouponModel>(destinationCoupon);

        couponModel.ProductName.Should().BeSameAs(destinationCoupon.ProductName);
        couponModel.Amount.Should().Be(destinationCoupon.Amount);

    }

    [Fact(Skip ="LATER")]
    public void Dicount_Create_Test()
    {
        _repository.Setup(x => x.CreateDiscount(It.IsAny<Coupon>())).ReturnsAsync(true);
    }
}