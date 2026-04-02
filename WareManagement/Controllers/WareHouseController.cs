using Microsoft.AspNetCore.Mvc;
using WareManagement.DTO.WarehouseDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
public class WareHouseController : BaseApiController
{
    private readonly IWareHouseService _wareHouseService;

    public WareHouseController(IWareHouseService wareHouseService)
    {
        _wareHouseService = wareHouseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 0)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (pageSize <= 0)
            {
                var list = await _wareHouseService.GetAllAsync(userId);
                return Ok(list);
            }

            var paged = await _wareHouseService.GetPagedAsync(userId, search, page, pageSize);
            return Ok(new { items = paged.Items, total = paged.Total, page, pageSize });
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
            var dto = await _wareHouseService.GetByIdAsync(userId, id);
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
    public async Task<IActionResult> Create([FromBody] CreateWarehouseRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var created = await _wareHouseService.CreateAsync(userId, request);
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var updated = await _wareHouseService.UpdateAsync(userId, id, request);
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
}
