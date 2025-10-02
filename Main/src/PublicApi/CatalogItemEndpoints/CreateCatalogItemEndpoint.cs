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
/// Creates a new Catalog Item
/// </summary>
public class CreateCatalogItemEndpoint : IEndpoint<IResult, CreateCatalogItemRequest, CatalogServiceClient>
{
    private readonly IUriComposer _uriComposer;

    public CreateCatalogItemEndpoint(IUriComposer uriComposer)
    {
        _uriComposer = uriComposer;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/catalog-items",
            [Authorize(Roles = BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async
            (CreateCatalogItemRequest request, CatalogServiceClient catalogServiceClient) =>
            {
                return await HandleAsync(request, catalogServiceClient);
            })
            .Produces<CreateCatalogItemResponse>()
            .WithTags("CatalogItemEndpoints");
    }

    public async Task<IResult> HandleAsync(CreateCatalogItemRequest request, CatalogServiceClient catalogServiceClient)
    {
        var response = new CreateCatalogItemResponse(request.CorrelationId());

        // Check if catalog item name already exists via microservice
        var nameExists = await catalogServiceClient.CheckCatalogItemNameExistsAsync(request.Name);
        if (nameExists)
        {
            return Results.BadRequest($"A catalogItem with name {request.Name} already exists");
        }

        // Create new item via microservice
        var newItemFromMicroservice = await catalogServiceClient.CreateCatalogItemAsync(
            request.Name,
            request.Description,
            request.Price,
            request.CatalogTypeId,
            request.CatalogBrandId,
            request.PictureUri ?? ""
        );

        // Map from microservice DTO to PublicApi DTO
        var dto = new CatalogItemDto
        {
            Id = newItemFromMicroservice.Id,
            CatalogBrandId = newItemFromMicroservice.CatalogBrandId,
            CatalogTypeId = newItemFromMicroservice.CatalogTypeId,
            Description = newItemFromMicroservice.Description,
            Name = newItemFromMicroservice.Name,
            PictureUri = _uriComposer.ComposePicUri(newItemFromMicroservice.PictureUri),
            Price = newItemFromMicroservice.Price
        };

        response.CatalogItem = dto;
        return Results.Created($"api/catalog-items/{dto.Id}", response);
    }
}

