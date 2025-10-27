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
        var response = await _httpClient.GetAsync($"api/basket/user/{userName}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Basket>();
    }

    public async Task<Basket> AddItemToBasketAsync(string username, int catalogItemId, decimal price, int quantity = 1)
    {
        var dto = new AddItemDto(username, catalogItemId, price, quantity);
        var response = await _httpClient.PostAsJsonAsync("api/basket/add-item", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Basket>();
    }

    public async Task<Basket> SetQuantitiesAsync(int basketId, Dictionary<string, int> quantities)
    {
        var dto = new SetQuantitiesDto(basketId, quantities);
        var response = await _httpClient.PostAsJsonAsync("api/basket/set-quantities", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Basket>();
    }

    public async Task DeleteBasketAsync(int basketId)
    {
        var response = await _httpClient.DeleteAsync($"api/basket/{basketId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task TransferBasketAsync(string anonymousId, string userName)
    {
        var dto = new TransferBasketDto(anonymousId, userName);
        var response = await _httpClient.PostAsJsonAsync("api/basket/transfer", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task<int> CountTotalBasketItemsAsync(string username)
    {
        var response = await _httpClient.GetAsync($"api/basket/{username}/item-count");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }
}

public record AddItemDto(string Username, int CatalogItemId, decimal Price, int Quantity = 1);
public record SetQuantitiesDto(int BasketId, Dictionary<string, int> Quantities);
public record TransferBasketDto(string AnonymousId, string UserName);
