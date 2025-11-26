using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using BlazorShared.Entities;

namespace BasketMS.Interfaces
{
    public interface IBasketRepository
    {
        Task<Basket> AddAsync(Basket entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Basket entity, CancellationToken cancellationToken = default);
        Task<Basket?> FirstOrDefaultAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default);
        Task<Basket?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        Task<List<Basket>> ListAsync(ISpecification<Basket> specification, CancellationToken cancellationToken = default);
        Task UpdateAsync(Basket entity, CancellationToken cancellationToken = default);
    }
}
