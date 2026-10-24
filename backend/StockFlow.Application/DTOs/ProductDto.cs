namespace StockFlow.Application.DTOs;

public class ProductDto
{
    public long Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
}