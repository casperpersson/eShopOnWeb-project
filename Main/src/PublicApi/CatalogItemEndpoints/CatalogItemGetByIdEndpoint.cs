using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints;

/// <summary>
/// Get a Catalog Item by Id
/// </summary>
public class CatalogItemGetByIdEndpoint : IEndpoint<IResult, GetByIdCatalogItemRequest, CatalogServiceClient>
{
    private readonly IUriComposer _uriComposer;

    public CatalogItemGetByIdEndpoint(IUriComposer uriComposer)
    {
        _uriComposer = uriComposer;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/catalog-items/{catalogItemId}",
            async (int catalogItemId, CatalogServiceClient catalogServiceClient) =>
            {
                return await HandleAsync(new GetByIdCatalogItemRequest(catalogItemId), catalogServiceClient);
            })
            .Produces<GetByIdCatalogItemResponse>()
            .WithTags("CatalogItemEndpoints");
    }

    public async Task<IResult> HandleAsync(GetByIdCatalogItemRequest request, CatalogServiceClient catalogServiceClient)
    {
        var response = new GetByIdCatalogItemResponse(request.CorrelationId());

        // Get item from microservice instead of repository
        var itemFromMicroservice = await catalogServiceClient.GetCatalogItemByIdAsync(request.CatalogItemId);
        if (itemFromMicroservice is null)
            return Results.NotFound();

        // Map from microservice DTO to PublicApi DTO
        response.CatalogItem = new CatalogItemDto
        {
            Id = itemFromMicroservice.Id,
            CatalogBrandId = itemFromMicroservice.CatalogBrandId,
            CatalogTypeId = itemFromMicroservice.CatalogTypeId,
            Description = itemFromMicroservice.Description,
            Name = itemFromMicroservice.Name,
            PictureUri = _uriComposer.ComposePicUri(itemFromMicroservice.PictureUri),
            Price = itemFromMicroservice.Price
        };

        return Results.Ok(response);
    }
}

