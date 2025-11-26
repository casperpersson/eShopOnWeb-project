using BlazorShared.Entities;
using NSubstitute;
using Xunit;
using BasketMS.Services;
using BasketMS.Specifications;
using BasketMS.Interfaces;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.BasketServiceTests;

public class AddItemToBasket
{
    private readonly string _buyerId = "Test buyerId";
    private readonly IBasketRepository _mockBasketRepo;
    private readonly IAppLogger<BasketService> _mockLogger = Substitute.For<IAppLogger<BasketService>>();

    public AddItemToBasket()
    {
        _mockBasketRepo = Substitute.For<IBasketRepository>();
    }

    [Fact]
    public async Task InvokesBasketRepositoryGetBySpecAsyncOnce()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(1, 1.5m);

        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);

        var basketService = new BasketService(_mockBasketRepo);

        await basketService.AddItemToBasketAsync(basket.BuyerId, 1, 1.50m);

        await _mockBasketRepo.Received().FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default);
    }

    [Fact]
    public async Task InvokesBasketRepositoryUpdateAsyncOnce()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(1, 1.1m, 1);
        _mockBasketRepo.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>(), default).Returns(basket);

        var basketService = new BasketService(_mockBasketRepo);

        await basketService.AddItemToBasketAsync(basket.BuyerId, 1, 1.50m);

        await _mockBasketRepo.Received().UpdateAsync(basket, default);
    }
}
