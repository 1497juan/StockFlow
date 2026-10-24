namespace StockFlow.Domain.Entities;

public class InventoryMovement
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public long UserId { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}