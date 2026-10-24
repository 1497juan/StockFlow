using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.DTOs;
using StockFlow.Application.Services.Interfaces;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController: ControllerBase
{
    private readonly IInventoryService _inventoryService;
    
    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCurrentInventory(CancellationToken cancellationToken)
    {
        var inventory = await _inventoryService.GetCurrentInventoryAsync(cancellationToken);

        return Ok(inventory);
    }

    [HttpPost("entries")]
    public async Task<IActionResult> RegisterEntry([FromBody] RegisterInventoryMovementDto request, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
            return Unauthorized();
        

        await _inventoryService.RegisterEntryAsync(request, userId.Value, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Entrada de inventario registrada correctamente."
        });
    }

    [HttpPost("exits")]
    public async Task<IActionResult> RegisterExit([FromBody] RegisterInventoryMovementDto request, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
            return Unauthorized();

        await _inventoryService.RegisterExitAsync(request, userId.Value, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Salida de inventario registrada correctamente."
        });
    }

    private long? GetAuthenticatedUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return long.TryParse(userIdClaim, out var userId) && userId > 0
            ? userId
            : null;
    }
}