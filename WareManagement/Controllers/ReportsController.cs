using Microsoft.AspNetCore.Mvc;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var userId = GetCurrentUserId();
            var dto = await _reportService.GetDashboardAsync(userId);
            return Ok(dto);
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

    [HttpGet("nxt")]
    public async Task<IActionResult> Nxt([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int? warehouseId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var rows = await _reportService.GetNxtAsync(userId, from, to, warehouseId);
            return Ok(rows);
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

    [HttpGet("product/{productId:int}/transactions")]
    public async Task<IActionResult> ProductTransactions(int productId, [FromQuery] int? warehouseId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var rows = await _reportService.GetProductTransactionHistoryAsync(userId, productId, warehouseId);
            return Ok(rows);
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

    [HttpGet("low-stock")]
    public async Task<IActionResult> LowStock([FromQuery] int? warehouseId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var rows = await _reportService.GetLowStockAsync(userId, warehouseId);
            return Ok(rows);
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

    [HttpGet("partner/{partnerId:int}/receipts")]
    public async Task<IActionResult> PartnerReceipts(int partnerId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var rows = await _reportService.GetReceiptsByPartnerAsync(userId, partnerId);
            return Ok(rows);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Problem();
        }
    }
}
