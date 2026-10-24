using StockFlow.Application.DTOs;
using StockFlow.Application.Repositories;
using StockFlow.Application.Services.Interfaces;

namespace StockFlow.Application.Services;

public class InventoryService: IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;

    public InventoryService(IInventoryRepository inventoryRepository, IProductRepository productRepository)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> GetCurrentInventoryAsync(CancellationToken cancellationToken = default)
    {
        var products = await _inventoryRepository.GetCurrentInventoryAsync(cancellationToken);

        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            Stock = product.Stock
        }).ToList();
    }

    public async Task RegisterEntryAsync(RegisterInventoryMovementDto request, long userId, CancellationToken cancellationToken = default)
    {
        ValidateMovement(request, userId);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
            throw new KeyNotFoundException($"No existe el producto con ID {request.ProductId}.");
        

        await _inventoryRepository.RegisterEntryAsync(request.ProductId, request.Quantity, userId, cancellationToken);
    }

    public async Task RegisterExitAsync(RegisterInventoryMovementDto request, long userId, CancellationToken cancellationToken = default)
    {
        ValidateMovement(request, userId);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
            throw new KeyNotFoundException($"No existe el producto con ID {request.ProductId}.");
        

        await _inventoryRepository.RegisterExitAsync(request.ProductId, request.Quantity, userId, cancellationToken);
    }

    private static void ValidateMovement(RegisterInventoryMovementDto request, long userId)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ProductId <= 0)
            throw new ArgumentException("El ID del producto debe ser mayor que cero.");
        

        if (request.Quantity <= 0) 
            throw new ArgumentException("La cantidad debe ser mayor que cero.");
        

        if (userId <= 0)
            throw new ArgumentException("El usuario debe ser válido.");
        
    }
}