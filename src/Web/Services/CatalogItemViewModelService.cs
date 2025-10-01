using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.HttpClients;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.ViewModels;

namespace Microsoft.eShopWeb.Web.Services;

public class CatalogItemViewModelService : ICatalogItemViewModelService
{
    private readonly CatalogServiceClient _catalogServiceClient;

    public CatalogItemViewModelService(CatalogServiceClient catalogServiceClient)
    {
        _catalogServiceClient = catalogServiceClient;
    }

    public async Task UpdateCatalogItem(CatalogItemViewModel viewModel)
    {
        Guard.Against.Null(viewModel, nameof(viewModel));
        Guard.Against.NullOrEmpty(viewModel.Name, nameof(viewModel.Name));

        // Get the existing catalog item from microservice
        var existingCatalogItem = await _catalogServiceClient.GetCatalogItemByIdAsync(viewModel.Id);
        Guard.Against.Null(existingCatalogItem, nameof(existingCatalogItem));


        throw new NotImplementedException(
            "UpdateCatalogItem requires implementing an update endpoint in the ProductCatalogMS microservice. " +
            "Add a PUT endpoint in CatalogItemController to handle updates.");
    }
}

