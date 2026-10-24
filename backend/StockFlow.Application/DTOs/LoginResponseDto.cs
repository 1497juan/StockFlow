namespace StockFlow.Application.DTOs;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public string TokenType { get; set; } = "Bearer";
}