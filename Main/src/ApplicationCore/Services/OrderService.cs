using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUriComposer _uriComposer;
    private readonly IRepository<Basket> _basketRepository;
    private readonly CatalogServiceClient _catalogServiceClient;

    public OrderService(IRepository<Basket> basketRepository,
        CatalogServiceClient catalogServiceClient,
        IRepository<Order> orderRepository,
        IUriComposer uriComposer)
    {
        _orderRepository = orderRepository;
        _uriComposer = uriComposer;
        _basketRepository = basketRepository;
        _catalogServiceClient = catalogServiceClient;
    }

    public async Task CreateOrderAsync(int basketId, Address shippingAddress)
    {
        var basketSpec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

        Guard.Against.Null(basket, nameof(basket));
        Guard.Against.EmptyBasketOnCheckout(basket.Items);

        // Get catalog items from microservice instead of repository
        var catalogItemIds = basket.Items.Select(item => item.CatalogItemId).Distinct().ToList();
        var catalogItems = new List<CatalogItemDto>();

        foreach (var itemId in catalogItemIds)
        {
            var catalogItemDto = await _catalogServiceClient.GetCatalogItemByIdAsync(itemId);
            if (catalogItemDto != null)
            {
                catalogItems.Add(catalogItemDto);
            }
        }

        var items = basket.Items.Select(basketItem =>
        {
            var catalogItemDto = catalogItems.First(c => c.Id == basketItem.CatalogItemId);
            var itemOrdered = new CatalogItemOrdered(catalogItemDto.Id, catalogItemDto.Name, _uriComposer.ComposePicUri(catalogItemDto.PictureUri));
            var orderItem = new OrderItem(itemOrdered, basketItem.UnitPrice, basketItem.Quantity);
            return orderItem;
        }).ToList();

        var order = new Order(basket.BuyerId, shippingAddress, items);

        await _orderRepository.AddAsync(order);
    }
}
