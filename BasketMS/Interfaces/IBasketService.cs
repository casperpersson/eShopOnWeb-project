using BasketMS.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorShared.Entities;

public interface IBasketService
{
    Task<Basket?> GetOrCreateBasketForUserAsync(string userName);
    Task<Basket> AddItemToBasketAsync(string username, int catalogItemId, decimal price, int quantity = 1);
    Task<Basket> SetQuantitiesAsync(int basketId, Dictionary<string, int> quantities);
    Task DeleteBasketAsync(int basketId);
    Task TransferBasketAsync(string anonymousId, string userName);
    Task<int> CountTotalBasketItemsAsync(string username);
}
