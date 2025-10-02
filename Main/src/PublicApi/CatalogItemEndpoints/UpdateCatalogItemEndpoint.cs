using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints;

/// <summary>
/// Updates a Catalog Item
/// </summary>
public class UpdateCatalogItemEndpoint : IEndpoint<IResult, UpdateCatalogItemRequest, CatalogServiceClient>
{
    private readonly IUriComposer _uriComposer;

    public UpdateCatalogItemEndpoint(IUriComposer uriComposer)
    {
        _uriComposer = uriComposer;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("api/catalog-items",
            [Authorize(Roles = BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async
            (UpdateCatalogItemRequest request, CatalogServiceClient catalogServiceClient) =>
            {
                return await HandleAsync(request, catalogServiceClient);
            })
            .Produces<UpdateCatalogItemResponse>()
            .WithTags("CatalogItemEndpoints");
    }

    public async Task<IResult> HandleAsync(UpdateCatalogItemRequest request, CatalogServiceClient catalogServiceClient)
    {
        var response = new UpdateCatalogItemResponse(request.CorrelationId());

        // Check if the item exists via microservice
        var existingItem = await catalogServiceClient.GetCatalogItemByIdAsync(request.Id);
        if (existingItem == null)
        {
            return Results.NotFound();
        }

        // Update item details via microservice
        await catalogServiceClient.UpdateCatalogItemDetailsAsync(request.Id, request.Name, request.Description, request.Price);

        // Update brand if changed
        if (existingItem.CatalogBrandId != request.CatalogBrandId)
        {
            await catalogServiceClient.UpdateCatalogItemBrandAsync(request.Id, request.CatalogBrandId);
        }

        // Update type if changed
        if (existingItem.CatalogTypeId != request.CatalogTypeId)
        {
            await catalogServiceClient.UpdateCatalogItemTypeAsync(request.Id, request.CatalogTypeId);
        }

        // Get the updated item to return in response
        var updatedItem = await catalogServiceClient.GetCatalogItemByIdAsync(request.Id);
        if (updatedItem == null)
        {
            return Results.Problem("Failed to retrieve updated item");
        }

        // Map from microservice DTO to PublicApi DTO
        var dto = new CatalogItemDto
        {
            Id = updatedItem.Id,
            CatalogBrandId = updatedItem.CatalogBrandId,
            CatalogTypeId = updatedItem.CatalogTypeId,
            Description = updatedItem.Description,
            Name = updatedItem.Name,
            PictureUri = _uriComposer.ComposePicUri(updatedItem.PictureUri),
            Price = updatedItem.Price
        };

        response.CatalogItem = dto;
        return Results.Ok(response);
    }
}


