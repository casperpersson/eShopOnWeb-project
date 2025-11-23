//using System.Collections.Generic;
//using System.Threading.Tasks;
//using BasketMS.Repository;
//using BasketMS.Services;
//using BlazorShared.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.eShopWeb.ApplicationCore.Interfaces;
//using Microsoft.eShopWeb.ApplicationCore.Services;
//using Microsoft.eShopWeb.Infrastructure.Data;
//using Microsoft.eShopWeb.UnitTests.Builders;
//using Xunit;

//namespace Microsoft.eShopWeb.IntegrationTests.Repositories.BasketRepositoryTests;

//public class SetQuantities
//{
//    private readonly CatalogContext _catalogContext;
//    private readonly BasketRepository _basketRepository;
//    private readonly BasketBuilder BasketBuilder = new BasketBuilder();

//    public SetQuantities()
//    {
//        var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
//            .UseInMemoryDatabase(databaseName: "TestCatalog")
//            .Options;
//        _catalogContext = new CatalogContext(dbOptions);
        
//    }

//    [Fact]
//    public async Task RemoveEmptyQuantities()
//    {
//        var basket = BasketBuilder.WithOneBasketItem();
//        var basketService = new BasketService(_basketRepository);
//        await _basketRepository.AddAsync(basket);
//        _catalogContext.SaveChanges();

//        await basketService.SetQuantitiesAsync(basket.BuyerId, new Dictionary<string, int>() { { BasketBuilder.BasketId.ToString(), 0 } });

//        Assert.Equal(0, basket.Items.Count);
//    }
//}
