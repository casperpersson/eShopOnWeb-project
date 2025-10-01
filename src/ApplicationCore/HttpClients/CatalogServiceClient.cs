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


