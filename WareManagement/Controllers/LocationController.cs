using Microsoft.AspNetCore.Mvc;
using WareManagement.DTO.LocationDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
public class LocationController : BaseApiController
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("warehouse/{warehouseId:int}")]
    public async Task<IActionResult> GetByWarehouse(int warehouseId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var list = await _locationService.GetByWarehouseAsync(userId, warehouseId);
            return Ok(list);
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
    public async Task<IActionResult> Create([FromBody] CreateLocationRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var created = await _locationService.CreateAsync(userId, request);
            return Ok(created);
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLocationRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var updated = await _locationService.UpdateAsync(userId, id, request);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _locationService.DeleteAsync(userId, id);
            return Ok(new { message = "Đã xóa vị trí" });
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
