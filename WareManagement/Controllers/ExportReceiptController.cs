using Microsoft.AspNetCore.Mvc;
using WareManagement.DTO.ExportReceiptDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
public class ExportReceiptController : BaseApiController
{
    private readonly IExportReceiptService _exportReceiptService;

    public ExportReceiptController(IExportReceiptService exportReceiptService)
    {
        _exportReceiptService = exportReceiptService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int? warehouseId, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _exportReceiptService.GetPagedAsync(userId, warehouseId, status, page, pageSize);
            return Ok(new { items = result.Items, total = result.Total, page, pageSize });
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var dto = await _exportReceiptService.GetByIdAsync(userId, id);
            return Ok(dto);
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExportReceiptRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var created = await _exportReceiptService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExportReceiptRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var updated = await _exportReceiptService.UpdateAsync(userId, id, request);
            return Ok(updated);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    [HttpPost("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var dto = await _exportReceiptService.ConfirmAsync(userId, id);
            return Ok(dto);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _exportReceiptService.CancelAsync(userId, id);
            return Ok(new { message = "Đã hủy phiếu xuất" });
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Problem();
        }
    }
}
