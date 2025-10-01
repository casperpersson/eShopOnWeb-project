using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints;

/// <summary>
/// Deletes a Catalog Item
/// </summary>
public class DeleteCatalogItemEndpoint : IEndpoint<IResult, DeleteCatalogItemRequest, CatalogServiceClient>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/catalog-items/{catalogItemId}",
            [Authorize(Roles = BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async
            (int catalogItemId, CatalogServiceClient catalogServiceClient) =>
            {
                return await HandleAsync(new DeleteCatalogItemRequest(catalogItemId), catalogServiceClient);
            })
            .Produces<DeleteCatalogItemResponse>()
            .WithTags("CatalogItemEndpoints");
    }

    public async Task<IResult> HandleAsync(DeleteCatalogItemRequest request, CatalogServiceClient catalogServiceClient)
    {
        var response = new DeleteCatalogItemResponse(request.CorrelationId());

        // Delete item via microservice
        var deleted = await catalogServiceClient.DeleteCatalogItemAsync(request.CatalogItemId);
        if (!deleted)
            return Results.NotFound();

        response.Status = "Deleted";
        return Results.Ok(response);
    }
}

