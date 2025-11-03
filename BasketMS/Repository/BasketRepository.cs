using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using BlazorShared.Entities;
using BasketMS.Interfaces;
using BasketMS.Specifications;
using BasketMS.Redis;

namespace BasketMS.Repository;

public class BasketRepository(RedisCache redisCache)
{
    private readonly RedisCache _cache = redisCache;
    private readonly ConcurrentDictionary<int, Basket> _baskets = new();
    private int _nextId = 1;

    public Task<Basket> AddAsync(Basket entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id == 0)
            entity.Id = Interlocked.Increment(ref _nextId);
        _cache.SetCachedData(entity.BuyerId, entity, TimeSpan.FromMinutes(10));
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(Basket entity, CancellationToken cancellationToken = default)
    {
        _baskets.TryRemove(entity.Id, out _);
        return Task.CompletedTask;
    }

    public Task<Basket?> FirstOrDefaultAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // For demo: just return the first basket matching BuyerId if spec is BasketWithItemsSpecification
        if (specification is BasketWithItemsSpecification spec)
        {
            if (spec.BuyerId != null)
            {
                Basket _basket = _cache.GetCachedDataByBuyerId<Basket>(spec.BuyerId);
                return Task.FromResult(_basket);
            }
                
            else
            {
                Basket _basket = _cache.GetCachedDataById<Basket>(spec.BasketId.ToString());
                return Task.FromResult(_basket);
            }
        }
        return Task.FromResult(_baskets.Values.FirstOrDefault());
    }

    public Task<Basket?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        if (id is int intId && _baskets.TryGetValue(intId, out var basket))
            return Task.FromResult<Basket?>(basket);
        return Task.FromResult<Basket?>(null);
    }

    public Task<List<Basket>> ListAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // For demo: just return all baskets
        return Task.FromResult(_baskets.Values.ToList());
    }

    public Task UpdateAsync(Basket entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id == 0)
            entity.Id = Interlocked.Increment(ref _nextId);
        _cache.SetCachedData(entity.BuyerId, entity, TimeSpan.FromMinutes(10));
        return Task.CompletedTask;
    }
}
