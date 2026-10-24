using StockFlow.Application.DTOs;

namespace StockFlow.Application.Services.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyList<ProductDto>> GetCurrentInventoryAsync(CancellationToken cancellationToken = default);

    Task RegisterEntryAsync(RegisterInventoryMovementDto request, long userId,
        CancellationToken cancellationToken = default);

    Task RegisterExitAsync(RegisterInventoryMovementDto request, long userId,
        CancellationToken cancellationToken = default);
}