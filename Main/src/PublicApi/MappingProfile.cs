using AutoMapper;

namespace Microsoft.eShopWeb.PublicApi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // AutoMapper mappings removed as endpoints now use manual mapping 
        // between microservice DTOs and PublicApi DTOs to avoid entity conflicts.
        // Manual mapping provides better control over the transformation
        // and avoids dependencies on monolith entities.
    }
}

