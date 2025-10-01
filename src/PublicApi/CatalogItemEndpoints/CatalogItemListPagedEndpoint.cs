using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints;

/// <summary>
/// List Catalog Items (paged)
/// </summary>
public class CatalogItemListPagedEndpoint : IEndpoint<IResult, ListPagedCatalogItemRequest, CatalogServiceClient>
{
    private readonly IUriComposer _uriComposer;
    private readonly IMapper _mapper;

    public CatalogItemListPagedEndpoint(IUriComposer uriComposer, IMapper mapper)
    {
        _uriComposer = uriComposer;
        _mapper = mapper;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/catalog-items",
            async (int? pageSize, int? pageIndex, int? catalogBrandId, int? catalogTypeId, CatalogServiceClient catalogServiceClient) =>
            {
                return await HandleAsync(new ListPagedCatalogItemRequest(pageSize, pageIndex, catalogBrandId, catalogTypeId), catalogServiceClient);
            })
            .Produces<ListPagedCatalogItemResponse>()
            .WithTags("CatalogItemEndpoints");
    }

    public async Task<IResult> HandleAsync(ListPagedCatalogItemRequest request, CatalogServiceClient catalogServiceClient)
    {
        await Task.Delay(1000); // Keep existing delay if needed for testing
        var response = new ListPagedCatalogItemResponse(request.CorrelationId());

        // Get all items from microservice
        var allItemsFromMicroservice = await catalogServiceClient.GetCatalogItemsAsync();

        // Apply filtering in memory (since microservice doesn't support filtering yet)
        var filteredItems = allItemsFromMicroservice.AsEnumerable();

        if (request.CatalogBrandId.HasValue)
        {
            filteredItems = filteredItems.Where(i => i.CatalogBrandId == request.CatalogBrandId.Value);
        }

        if (request.CatalogTypeId.HasValue)
        {
            filteredItems = filteredItems.Where(i => i.CatalogTypeId == request.CatalogTypeId.Value);
        }

        int totalItems = filteredItems.Count();

        // Apply pagination
        var pagedItems = filteredItems
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Map from microservice DTOs to PublicApi DTOs
        response.CatalogItems.AddRange(pagedItems.Select(itemFromMs => new CatalogItemDto
        {
            Id = itemFromMs.Id,
            Name = itemFromMs.Name,
            Description = itemFromMs.Description,
            Price = itemFromMs.Price,
            PictureUri = _uriComposer.ComposePicUri(itemFromMs.PictureUri),
            CatalogTypeId = itemFromMs.CatalogTypeId,
            CatalogBrandId = itemFromMs.CatalogBrandId
        }));

        // Calculate page count
        if (request.PageSize > 0)
        {
            response.PageCount = int.Parse(Math.Ceiling((decimal)totalItems / request.PageSize).ToString());
        }
        else
        {
            response.PageCount = totalItems > 0 ? 1 : 0;
        }

        return Results.Ok(response);
    }
}

