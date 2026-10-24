using StockFlow.Domain.Entities;

namespace StockFlow.Application.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}