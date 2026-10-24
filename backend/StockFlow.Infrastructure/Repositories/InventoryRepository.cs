using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Repositories;
using StockFlow.Domain.Entities;
using StockFlow.Infrastructure.Persistence;

namespace StockFlow.Infrastructure.Repositories;

public class InventoryRepository: IInventoryRepository
{
    private readonly AppDbContext _context;

    public InventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegisterEntryAsync(long productId, int quantity, long userId, CancellationToken cancellationToken = default)
    {
        await ExecuteProcedureAsync(
            "CALL sp_register_inventory_entry(@ProductId, @Quantity, @UserId)",
            productId,
            quantity,
            userId,
            cancellationToken);
    }

    public async Task RegisterExitAsync(long productId, int quantity, long userId, CancellationToken cancellationToken = default)
    {
        await ExecuteProcedureAsync(
            "CALL sp_register_inventory_exit(@ProductId, @Quantity, @UserId)",
            productId,
            quantity,
            userId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetCurrentInventoryAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FromSqlRaw("SELECT * FROM fn_get_current_inventory()")
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryMovement>> GetMovementsAsync(CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            var command = new CommandDefinition(
                """
                SELECT
                    id AS "Id",
                    product_id AS "ProductId",
                    user_id AS "UserId",
                    movement_type AS "MovementType",
                    quantity AS "Quantity",
                    created_at AS "CreatedAt"
                FROM inventory_movements
                ORDER BY created_at DESC, id DESC
                """,
                cancellationToken: cancellationToken);

            var movements = await connection.QueryAsync<InventoryMovement>(command);

            return movements.AsList();
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }

    private async Task ExecuteProcedureAsync(string sql, long productId, int quantity, long userId, CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            var command = new CommandDefinition(
                sql,
                new
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UserId = userId
                },
                cancellationToken: cancellationToken);

            await connection.ExecuteAsync(command);
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }
}