using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using BlazorShared.Interfaces;

namespace BlazorShared.Entities;

public class Basket : BaseEntity, IAggregateRoot
{
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; } = new();

    public int TotalItems => Items.Sum(i => i.Quantity);
    public Basket(string buyerId)
    {
        BuyerId = buyerId;
    }

    public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1)
    {
        if (!Items.Any(i => i.CatalogItemId == catalogItemId))
        {
            Items.Add(new BasketItem(catalogItemId, quantity, unitPrice));
            return;
        }
        var existingItem = Items.First(i => i.CatalogItemId == catalogItemId);
        existingItem.AddQuantity(quantity);
    }

    public void RemoveEmptyItems()
    {
        Items.RemoveAll(i => i.Quantity == 0);
    }

    public void SetNewBuyerId(string buyerId)
    {
        BuyerId = buyerId;
    }
}
