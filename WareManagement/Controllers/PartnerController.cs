using Microsoft.AspNetCore.Mvc;
using WareManagement.DTO.PartnerDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
public class PartnerController : BaseApiController
{
    private readonly IPartnerService _partnerService;

    public PartnerController(IPartnerService partnerService)
    {
        _partnerService = partnerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] string? type, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _partnerService.GetPagedAsync(userId, type, search, page, pageSize);
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
            var dto = await _partnerService.GetByIdAsync(userId, id);
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
    public async Task<IActionResult> Create([FromBody] CreatePartnerRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var created = await _partnerService.CreateAsync(userId, request);
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartnerRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var updated = await _partnerService.UpdateAsync(userId, id, request);
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
            await _partnerService.DeleteAsync(userId, id);
            return Ok(new { message = "Đã xóa đối tác" });
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
