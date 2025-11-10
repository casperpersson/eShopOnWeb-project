using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using BasketMS.Entities;
using BasketMS.Interfaces;

namespace BasketMS.Repository;

public class BasketRepository : IRepository<Basket>
{
    private readonly ConcurrentDictionary<int, Basket> _baskets = new();
    private int _nextId = 1;

    public Task<Basket?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        _baskets.TryGetValue((int)(object)id, out var basket);
        return Task.FromResult(basket);
    }

    public Task<Basket> AddAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        basket.Id = id;
        _baskets[id] = basket;
        return Task.FromResult(basket);
    }

    public Task<IEnumerable<Basket>> AddRangeAsync(IEnumerable<Basket> entities, CancellationToken cancellationToken = default)
    {
        foreach (var basket in entities)
        {
            var id = Interlocked.Increment(ref _nextId);
            basket.Id = id;
            _baskets[id] = basket;
        }
        return Task.FromResult(entities);
    }

    public Task UpdateAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        _baskets[basket.Id] = basket;
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<Basket> entities, CancellationToken cancellationToken = default)
    {
        foreach (var basket in entities)
        {
            _baskets[basket.Id] = basket;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        _baskets.TryRemove(basket.Id, out _);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<Basket> entities, CancellationToken cancellationToken = default)
    {
        foreach (var basket in entities)
        {
            _baskets.TryRemove(basket.Id, out _);
        }
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Since this is an in-memory repository, no actual persistence is done.
        return Task.FromResult(0);
    }

    public Task<T?> GetBySpecAsync<T>(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(default(T));
    }

    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<Basket, TResult> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(default(TResult));
    }

    public Task<Basket?> FirstOrDefaultAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(default(Basket));
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<Basket, TResult> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(default(TResult));
    }

    public Task<Basket?> SingleOrDefaultAsync(ISingleResultSpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(default(Basket));
    }

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<Basket, TResult> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(default(TResult));
    }

    public Task<List<Basket>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_baskets.Values.ToList());
    }

    public Task<List<Basket>> ListAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(new List<Basket>());
    }

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<Basket, TResult> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(new List<TResult>());
    }

    public Task<int> CountAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(0);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_baskets.Count);
    }

    public Task<bool> AnyAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return Task.FromResult(false);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_baskets.Any());
    }

    public IAsyncEnumerable<Basket> AsAsyncEnumerable(ISpecification<Basket> specification)
    {
        // Implementation for specification pattern is omitted for simplicity.
        return AsyncEnumerable.Empty<Basket>();
    }
}
