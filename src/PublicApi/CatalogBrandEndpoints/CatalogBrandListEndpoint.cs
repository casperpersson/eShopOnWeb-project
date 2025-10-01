using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi.CatalogBrandEndpoints;

/// <summary>
/// List Catalog Brands
/// </summary>
public class CatalogBrandListEndpoint : IEndpoint<IResult, CatalogServiceClient>
{
    private readonly IMapper _mapper;

    public CatalogBrandListEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/catalog-brands",
            async (CatalogServiceClient catalogServiceClient) =>
            {
                return await HandleAsync(catalogServiceClient);
            })
           .Produces<ListCatalogBrandsResponse>()
           .WithTags("CatalogBrandEndpoints");
    }

    public async Task<IResult> HandleAsync(CatalogServiceClient catalogServiceClient)
    {
        var response = new ListCatalogBrandsResponse();

        // Get brands from microservice instead of repository
        var brandsFromMicroservice = await catalogServiceClient.GetCatalogBrandsAsync();

        // Map from microservice DTOs to PublicApi DTOs
        response.CatalogBrands.AddRange(brandsFromMicroservice.Select(brandFromMs =>
            new CatalogBrandDto
            {
                Id = brandFromMs.Id,
                Name = brandFromMs.Brand  // Note: microservice DTO has 'Brand', PublicApi DTO has 'Name'
            }));

        return Results.Ok(response);
    }
}
