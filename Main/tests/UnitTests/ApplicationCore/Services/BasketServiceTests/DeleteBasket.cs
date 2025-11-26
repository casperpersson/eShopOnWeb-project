using BasketMS.Interfaces;
using BasketMS.Services;
using BlazorShared.Entities;
using NSubstitute;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.BasketServiceTests;

public class DeleteBasket
{
    private readonly string _buyerId = "Test buyerId";
    private readonly IBasketRepository _mockBasketRepo = Substitute.For<IBasketRepository>();
    private readonly IAppLogger<IBasketService> _mockLogger = Substitute.For<IAppLogger<IBasketService>>();

    [Fact]
    public async Task ShouldInvokeBasketRepositoryDeleteAsyncOnce()
    {
        var basket = new Basket(_buyerId);
        basket.AddItem(1, 1.1m, 1);
        basket.AddItem(2, 1.1m, 1);
        _mockBasketRepo.GetByIdAsync(Arg.Any<int>(), default)
            .Returns(basket);
        var basketService = new BasketService(_mockBasketRepo);

        await basketService.DeleteBasketAsync(1);

        await _mockBasketRepo.Received().DeleteAsync(Arg.Any<Basket>(), default);
    }
}
