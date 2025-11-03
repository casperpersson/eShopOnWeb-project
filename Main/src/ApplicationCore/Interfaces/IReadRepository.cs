using Ardalis.Specification;
using BlazorShared.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}
