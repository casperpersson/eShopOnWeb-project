using Ardalis.Specification;
using BlazorShared.Entities;

namespace BasketMS.Specifications;

public sealed class BasketWithItemsSpecification : Specification<Basket>
{
    public int? BasketId { get; }
    public string? BuyerId { get; }
    public BasketWithItemsSpecification(int basketId)
    {
        BasketId = basketId;
        Query
            .Where(b => b.Id == basketId)
            .Include(b => b.Items);
    }

    public BasketWithItemsSpecification(string buyerId)
    {
        buyerId = BuyerId;
        Query
            .Where(b => b.BuyerId == buyerId)
            .Include(b => b.Items);
    }
}
