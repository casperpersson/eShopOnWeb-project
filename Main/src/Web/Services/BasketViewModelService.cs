using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Pages.Basket;
using Microsoft.eShopWeb.Web.ViewModels;
using BlazorShared.Entities;

namespace Microsoft.eShopWeb.Web.Services;

public class BasketViewModelService : IBasketViewModelService
{
    private readonly BasketServiceClient _basketServiceClient;
    private readonly CatalogServiceClient _catalogServiceClient;
    private readonly IUriComposer _uriComposer;

    public BasketViewModelService(
        BasketServiceClient basketServiceClient,
        CatalogServiceClient catalogServiceClient,
        IUriComposer uriComposer)
    {
        _basketServiceClient = basketServiceClient;
        _catalogServiceClient = catalogServiceClient;
        _uriComposer = uriComposer;
    }

    public async Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
    {
        var basket = await _basketServiceClient.GetOrCreateBasketForUserAsync(userName);
        return await Map(basket);
    }

    public async Task<BasketViewModel> AddItemToBasket(string userName, int catalogItemId, decimal price, int quantity = 1)
    {
        var basket = await _basketServiceClient.AddItemToBasketAsync(userName, catalogItemId, price, quantity);
        return await Map(basket);
    }

    public async Task<BasketViewModel> SetQuantities(int basketId, Dictionary<string, int> quantities)
    {
        var basket = await _basketServiceClient.SetQuantitiesAsync(basketId, quantities);
        return await Map(basket);
    }

    public async Task DeleteBasket(int basketId)
    {
        await _basketServiceClient.DeleteBasketAsync(basketId);
    }

    public async Task TransferBasket(string anonymousId, string userName)
    {
        await _basketServiceClient.TransferBasketAsync(anonymousId, userName);
    }

    public async Task<BasketViewModel> Map(Basket basket)
    {
        var items = new List<BasketItemViewModel>();
        foreach (var item in basket.Items)
        {
            var catalogItem = await _catalogServiceClient.GetCatalogItemByIdAsync(item.CatalogItemId);
            items.Add(new BasketItemViewModel
            {
                Id = item.Id,
                CatalogItemId = item.CatalogItemId,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                ProductName = catalogItem?.Name,
                PictureUrl = _uriComposer.ComposePicUri(catalogItem?.PictureUri ?? "")
            });
        }
        return new BasketViewModel
        {
            Id = basket.Id,
            BuyerId = basket.BuyerId,
            Items = items
        };
    }

    public async Task<int> CountTotalBasketItems(string username)
    {
        return await _basketServiceClient.CountTotalBasketItemsAsync(username);
    }
}
