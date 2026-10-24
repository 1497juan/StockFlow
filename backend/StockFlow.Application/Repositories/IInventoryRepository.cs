using StockFlow.Domain.Entities;

namespace StockFlow.Application.Repositories;

public interface IInventoryRepository
{
    Task RegisterEntryAsync(long productId, int quantity, long userId, CancellationToken cancellationToken = default);
    Task RegisterExitAsync(long productId, int quantity, long userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetCurrentInventoryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryMovement>> GetMovementsAsync(CancellationToken cancellationToken = default);
}