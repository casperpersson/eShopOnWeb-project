using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.ApplicationCore.HttpClients;

public class CatalogServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogServiceClient> _logger;

    public CatalogServiceClient(HttpClient httpClient, ILogger<CatalogServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<CatalogItemDto>> GetCatalogItemsAsync()
    {
        _logger.LogInformation("Fetching catalog items from microservice");

        try
        {
            var response = await _httpClient.GetAsync("api/catalogitem");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<CatalogItemDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return items ?? Enumerable.Empty<CatalogItemDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog items from microservice");
            throw;
        }
    }

    public async Task<CatalogItemDto?> GetCatalogItemByIdAsync(int id)
    {
        _logger.LogInformation("Fetching catalog item {Id} from microservice", id);

        try
        {
            var response = await _httpClient.GetAsync($"api/catalogitem/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<CatalogItemDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog item {Id} from microservice", id);
            throw;
        }
    }

    public async Task<IEnumerable<CatalogBrandDto>> GetCatalogBrandsAsync()
    {
        _logger.LogInformation("Fetching catalog brands from microservice");

        try
        {
            var response = await _httpClient.GetAsync("api/catalogbrand");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var brands = JsonSerializer.Deserialize<List<CatalogBrandDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return brands ?? Enumerable.Empty<CatalogBrandDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog brands from microservice");
            throw;
        }
    }

    public async Task<IEnumerable<CatalogTypeDto>> GetCatalogTypesAsync()
    {
        _logger.LogInformation("Fetching catalog types from microservice");

        try
        {
            var response = await _httpClient.GetAsync("api/catalogtype");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var types = JsonSerializer.Deserialize<List<CatalogTypeDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return types ?? Enumerable.Empty<CatalogTypeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog types from microservice");
            throw;
        }
    }
    public async Task UpdateCatalogItemDetailsAsync(int id, string name, string description, decimal price)
    {
        _logger.LogInformation("Updating catalog item {Id} via microservice", id);

        try
        {
            var request = new
            {
                Name = name,
                Description = description,
                Price = price
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/catalogitem/{id}/details", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item {Id} via microservice", id);
            throw;
        }
    }

    public async Task<CatalogItemDto> CreateCatalogItemAsync(string name, string description, decimal price, int catalogTypeId, int catalogBrandId, string pictureUri = "")
    {
        _logger.LogInformation("Creating catalog item via microservice");

        try
        {
            var request = new
            {
                Name = name,
                Description = description,
                Price = price,
                CatalogTypeId = catalogTypeId,
                CatalogBrandId = catalogBrandId,
                PictureUri = pictureUri
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/catalogitem", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var createdItem = JsonSerializer.Deserialize<CatalogItemDto>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return createdItem ?? throw new InvalidOperationException("Failed to create catalog item");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating catalog item via microservice");
            throw;
        }
    }

    public async Task<bool> CheckCatalogItemNameExistsAsync(string name)
    {
        _logger.LogInformation("Checking if catalog item name {Name} exists via microservice", name);

        try
        {
            // Get all items and check if name exists (simple implementation)
            var allItems = await GetCatalogItemsAsync();
            return allItems.Any(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking catalog item name existence via microservice");
            throw;
        }
    }

    public async Task<bool> DeleteCatalogItemAsync(int id)
    {
        _logger.LogInformation("Deleting catalog item {Id} via microservice", id);

        try
        {
            var response = await _httpClient.DeleteAsync($"api/catalogitem/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting catalog item {Id} via microservice", id);
            throw;
        }
    }

    public async Task UpdateCatalogItemBrandAsync(int id, int catalogBrandId)
    {
        _logger.LogInformation("Updating catalog item {Id} brand via microservice", id);

        try
        {
            var request = new { CatalogBrandId = catalogBrandId };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/catalogitem/{id}/brand", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item {Id} brand via microservice", id);
            throw;
        }
    }

    public async Task UpdateCatalogItemTypeAsync(int id, int catalogTypeId)
    {
        _logger.LogInformation("Updating catalog item {Id} type via microservice", id);

        try
        {
            var request = new { CatalogTypeId = catalogTypeId };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/catalogitem/{id}/type", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item {Id} type via microservice", id);
            throw;
        }
    }




}

// DTOs for microservice communication
public record CatalogItemDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string PictureUri,
    int CatalogTypeId,
    int CatalogBrandId
);

public record CatalogBrandDto(int Id, string Brand);
public record CatalogTypeDto(int Id, string Type);


