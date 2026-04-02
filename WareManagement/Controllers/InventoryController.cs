using Microsoft.AspNetCore.Mvc;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
public class InventoryController : BaseApiController
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] int? warehouseId, [FromQuery] string? productCode, [FromQuery] string? productName)
    {
        try
        {
            var userId = GetCurrentUserId();
            var list = await _inventoryService.GetSummaryAsync(userId, warehouseId, productCode, productName);
            return Ok(list);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return Problem();
        }
    }
}
