using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi.CatalogTypeEndpoints;

/// <summary>
/// List Catalog Types
/// </summary>
public class CatalogTypeListEndpoint : IEndpoint<IResult, CatalogServiceClient>
{
    private readonly IMapper _mapper;

    public CatalogTypeListEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/catalog-types",
            async (CatalogServiceClient catalogServiceClient) =>
            {
                return await HandleAsync(catalogServiceClient);
            })
            .Produces<ListCatalogTypesResponse>()
            .WithTags("CatalogTypeEndpoints");
    }

    public async Task<IResult> HandleAsync(CatalogServiceClient catalogServiceClient)
    {
        var response = new ListCatalogTypesResponse();

        // Get types from microservice instead of repository
        var typesFromMicroservice = await catalogServiceClient.GetCatalogTypesAsync();

        // Map from microservice DTOs to PublicApi DTOs
        response.CatalogTypes.AddRange(typesFromMicroservice.Select(typeFromMs =>
            new CatalogTypeDto
            {
                Id = typeFromMs.Id,
                Name = typeFromMs.Type  // Note: microservice DTO has 'Type', PublicApi DTO has 'Name'
            }));

        return Results.Ok(response);
    }
}
