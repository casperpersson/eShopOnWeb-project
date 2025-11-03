using BlazorShared.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BasketMS.Redis
{
    public class RedisCache(IDistributedCache cache)
    {
        private readonly IDistributedCache? _cache = cache;

        public Basket GetCachedDataByBuyerId<Basket>(string key)
        {
            var jsonData = _cache.GetString(key);

            if (jsonData == null)
                return default(Basket);

            return JsonSerializer.Deserialize<Basket>(jsonData);
        }
        
        public Basket GetCachedDataById<Basket>(string key)
        {
            var jsonData = _cache.GetString(key);

            if (jsonData == null)
                return default(Basket);

            return JsonSerializer.Deserialize<Basket>(jsonData);
        }

        public void SetCachedData<T>(string key, T data, TimeSpan cacheDuration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };

            var jsonData = JsonSerializer.Serialize(data);
            _cache.SetString(key, jsonData, options);
        }
    }
}
