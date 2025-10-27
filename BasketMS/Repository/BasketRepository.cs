using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using BlazorShared.Entities;
using BasketMS.Interfaces;
using BasketMS.Specifications;

namespace BasketMS.Repository;

public class BasketRepository : IRepository<Basket>
{
    private readonly ConcurrentDictionary<int, Basket> _baskets = new();
    private int _nextId = 1;

    public Task<Basket> AddAsync(Basket entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id == 0)
            entity.Id = Interlocked.Increment(ref _nextId);
        _baskets[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Basket>> AddRangeAsync(IEnumerable<Basket> entities, CancellationToken cancellationToken = default)
    {
        var added = new List<Basket>();
        foreach (var entity in entities)
        {
            added.Add(AddAsync(entity, cancellationToken).Result);
        }
        return Task.FromResult<IEnumerable<Basket>>(added);
    }

    public Task<bool> AnyAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ListAsync(specification, cancellationToken).Result.Any());
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_baskets.Any());
    }

    public IAsyncEnumerable<Basket> AsAsyncEnumerable(ISpecification<Basket> specification)
    {
        var list = ListAsync(specification).Result;
        return ToAsyncEnumerable(list);
    }

    public static async IAsyncEnumerable<Basket> ToAsyncEnumerable(List<Basket> baskets)
    {
        foreach (var basket in baskets)
        {
            yield return basket;
            await Task.Yield(); // Optional: yields control to the caller
        }
    }


    public Task<int> CountAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ListAsync(specification, cancellationToken).Result.Count);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_baskets.Count);
    }

    public Task DeleteAsync(Basket entity, CancellationToken cancellationToken = default)
    {
        _baskets.TryRemove(entity.Id, out _);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<Basket> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            _baskets.TryRemove(entity.Id, out _);
        }
        return Task.CompletedTask;
    }

    public Task<Basket?> FirstOrDefaultAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // For demo: just return the first basket matching BuyerId if spec is BasketWithItemsSpecification
        if (specification is BasketWithItemsSpecification spec)
        {
            if (spec.BuyerId != null)
                return Task.FromResult(_baskets.Values.FirstOrDefault(b => b.BuyerId == spec.BuyerId));
            else
                return Task.FromResult(_baskets.Values.FirstOrDefault(b => b.Id == spec.BasketId));
        }
        return Task.FromResult(_baskets.Values.FirstOrDefault());
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<Basket, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Basket?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        if (id is int intId && _baskets.TryGetValue(intId, out var basket))
            return Task.FromResult<Basket?>(basket);
        return Task.FromResult<Basket?>(null);
    }

    public Task<Basket?> GetBySpecAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync(specification, cancellationToken);
    }

    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<Basket, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Basket>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_baskets.Values.ToList());
    }

    public Task<List<Basket>> ListAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        // For demo: just return all baskets
        return Task.FromResult(_baskets.Values.ToList());
    }

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<Basket, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // No-op for in-memory
        return Task.FromResult(0);
    }

    public Task<Basket?> SingleOrDefaultAsync(ISingleResultSpecification<Basket> specification, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync((ISpecification<Basket>)specification, cancellationToken);
    }

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<Basket, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Basket entity, CancellationToken cancellationToken = default)
    {
        _baskets[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<Basket> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            _baskets[entity.Id] = entity;
        }
        return Task.CompletedTask;
    }
}
