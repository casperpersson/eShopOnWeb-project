using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BlazorShared.Entities;
using BasketMS.Interfaces;
using BasketMS.Specifications;
using BasketMS.Repository;

namespace BasketMS.Services;

public class BasketService : IBasketService
{
    private readonly IRepository<Basket> _basketRepository;

    public BasketService(IRepository<Basket> basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Basket?> GetOrCreateBasketForUserAsync(string userName)
    {
        var basketSpec = new BasketWithItemsSpecification(userName);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

        if (basket == null)
        {
            basket = new Basket(userName);
            await _basketRepository.AddAsync(basket);
        }
        return basket;
    }

    public async Task<Basket> AddItemToBasketAsync(string username, int catalogItemId, decimal price, int quantity = 1)
    {
        var basketSpec = new BasketWithItemsSpecification(username);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

        if (basket == null)
        {
            basket = new Basket(username);
            await _basketRepository.AddAsync(basket);
        }

        basket.AddItem(catalogItemId, price, quantity);

        await _basketRepository.UpdateAsync(basket);
        return basket;
    }

    public async Task<Basket> SetQuantitiesAsync(int basketId, Dictionary<string, int> quantities)
    {
        var basketSpec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);
        Guard.Against.Null(basket, nameof(basket));

        foreach (var item in basket.Items)
        {
            if (quantities.TryGetValue(item.Id.ToString(), out var quantity))
            {
                item.SetQuantity(quantity);
            }
        }
        basket.RemoveEmptyItems();
        await _basketRepository.UpdateAsync(basket);
        return basket;
    }

    public async Task DeleteBasketAsync(int basketId)
    {
        var basket = await _basketRepository.GetByIdAsync(basketId);
        Guard.Against.Null(basket, nameof(basket));
        await _basketRepository.DeleteAsync(basket);
    }

    public async Task TransferBasketAsync(string anonymousId, string userName)
    {
        var anonymousBasketSpec = new BasketWithItemsSpecification(anonymousId);
        var anonymousBasket = await _basketRepository.FirstOrDefaultAsync(anonymousBasketSpec);
        if (anonymousBasket == null) return;
        var userBasketSpec = new BasketWithItemsSpecification(userName);
        var userBasket = await _basketRepository.FirstOrDefaultAsync(userBasketSpec);
        if (userBasket == null)
        {
            userBasket = new Basket(userName);
            await _basketRepository.AddAsync(userBasket);
        }
        foreach (var item in anonymousBasket.Items)
        {
            userBasket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity);
        }
        await _basketRepository.UpdateAsync(userBasket);
        await _basketRepository.DeleteAsync(anonymousBasket);
    }

    public async Task<int> CountTotalBasketItemsAsync(string username)
    {
        var basketSpec = new BasketWithItemsSpecification(username);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);
        return basket?.Items.Sum(i => i.Quantity) ?? 0;
    }
}
