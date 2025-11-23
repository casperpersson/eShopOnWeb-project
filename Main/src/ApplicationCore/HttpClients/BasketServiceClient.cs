using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorShared.Entities;

public class BasketServiceClient
{
    private readonly HttpClient _httpClient;

    public BasketServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Basket?> GetOrCreateBasketForUserAsync(string userName)
    {
        var response = await _httpClient.GetAsync($"user/{userName}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Basket>();
    }

    public async Task<Basket> AddItemToBasketAsync(string username, int catalogItemId, decimal price, int quantity = 1)
    {
        var dto = new AddItemDto(username, catalogItemId, price, quantity);
        var response = await _httpClient.PostAsJsonAsync("add-item", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Basket>();
    }

    public async Task<Basket> SetQuantitiesAsync(string username, Dictionary<string, int> quantities)
    {
        var dto = new SetQuantitiesDto(username, quantities);
        var response = await _httpClient.PostAsJsonAsync("set-quantities", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Basket>();
    }

    public async Task DeleteBasketAsync(int basketId)
    {
        var response = await _httpClient.DeleteAsync($"{basketId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task TransferBasketAsync(string anonymousId, string userName)
    {
        var dto = new TransferBasketDto(anonymousId, userName);
        var response = await _httpClient.PostAsJsonAsync("transfer", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task<int> CountTotalBasketItemsAsync(string username)
    {
        var response = await _httpClient.GetAsync($"{username}/item-count");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }
}

public record AddItemDto(string Username, int CatalogItemId, decimal Price, int Quantity = 1);
public record SetQuantitiesDto(string username, Dictionary<string, int> Quantities);
public record TransferBasketDto(string AnonymousId, string UserName);
