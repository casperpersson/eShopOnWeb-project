using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Web.Services;

public class MicroserviceCatalogViewModelService : ICatalogViewModelService
{
    private readonly ILogger<MicroserviceCatalogViewModelService> _logger;
    private readonly CatalogServiceClient _catalogServiceClient;
    private readonly IUriComposer _uriComposer;

    public MicroserviceCatalogViewModelService(
        ILogger<MicroserviceCatalogViewModelService> logger,
        CatalogServiceClient catalogServiceClient,
        IUriComposer uriComposer)
    {
        _logger = logger;
        _catalogServiceClient = catalogServiceClient;
        _uriComposer = uriComposer;
    }

    public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
    {
        _logger.LogInformation("GetCatalogItems called (microservice).");

        // Get all items from microservice
        var allItems = await _catalogServiceClient.GetCatalogItemsAsync();

        // Apply filtering
        var filteredItems = allItems.AsEnumerable();

        if (brandId.HasValue && brandId > 0)
        {
            filteredItems = filteredItems.Where(i => i.CatalogBrandId == brandId.Value);
        }

        if (typeId.HasValue && typeId > 0)
        {
            filteredItems = filteredItems.Where(i => i.CatalogTypeId == typeId.Value);
        }

        var totalItems = filteredItems.Count();

        // Apply pagination
        var itemsOnPage = filteredItems
            .Skip(itemsPage * pageIndex)
            .Take(itemsPage)
            .ToList();

        var vm = new CatalogIndexViewModel()
        {
            CatalogItems = itemsOnPage.Select(i => new CatalogItemViewModel()
            {
                Id = i.Id,
                Name = i.Name,
                PictureUri = _uriComposer.ComposePicUri(i.PictureUri),
                Price = i.Price
            }).ToList(),
            Brands = (await GetBrands()).ToList(),
            Types = (await GetTypes()).ToList(),
            BrandFilterApplied = brandId ?? 0,
            TypesFilterApplied = typeId ?? 0,
            PaginationInfo = new PaginationInfoViewModel()
            {
                ActualPage = pageIndex,
                ItemsPerPage = itemsOnPage.Count,
                TotalItems = totalItems,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / itemsPage)).ToString())
            }
        };

        vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
        vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

        return vm;
    }

    public async Task<IEnumerable<SelectListItem>> GetBrands()
    {
        _logger.LogInformation("GetBrands called (microservice).");
        var brands = await _catalogServiceClient.GetCatalogBrandsAsync();

        var items = brands
            .Select(brand => new SelectListItem() { Value = brand.Id.ToString(), Text = brand.Brand })
            .OrderBy(b => b.Text)
            .ToList();

        var allItem = new SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }

    public async Task<IEnumerable<SelectListItem>> GetTypes()
    {
        _logger.LogInformation("GetTypes called (microservice).");
        var types = await _catalogServiceClient.GetCatalogTypesAsync();

        var items = types
            .Select(type => new SelectListItem() { Value = type.Id.ToString(), Text = type.Type })
            .OrderBy(t => t.Text)
            .ToList();

        var allItem = new SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }
}

