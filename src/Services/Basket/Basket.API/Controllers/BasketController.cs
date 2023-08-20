using System.Net;
using Basket.API.Entities;
using Basket.API.GrpcService;
using Basket.API.Repositries;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly DiscountGrpcService _discountGrpcService;

    public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService)
    {
        _repository = repository;
        _discountGrpcService = discountGrpcService;
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
        var basket = await _repository.GetBasket(userName);
        return Ok(basket ?? new ShoppingCart(userName)); 
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody]ShoppingCart basket)
    {
        foreach(var item in basket.Items)
        {
            var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
            item.Price -= coupon.Amount;
        }

        return Ok(await _repository.UpdateBasket(basket));
    }

    [HttpDelete("{userName}", Name ="DeleteBasket")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
        await _repository.DeleteBasket(userName);
        return Ok();
    }

    
}