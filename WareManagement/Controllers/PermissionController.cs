using Microsoft.AspNetCore.Mvc;
using WareManagement.DTO.PermissionDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionController : BaseApiController
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var adminId = GetCurrentUserId();
            var list = await _permissionService.GetAllAsync(adminId);
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var adminId = GetCurrentUserId();
            var dto = await _permissionService.GetByIdAsync(adminId, id);
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
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequestDto request)
    {
        if (request is null) return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var adminId = GetCurrentUserId();
            var created = await _permissionService.CreateAsync(adminId, request);
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePermissionRequestDto request)
    {
        if (request is null) return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var adminId = GetCurrentUserId();
            var updated = await _permissionService.UpdateAsync(adminId, id, request);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var adminId = GetCurrentUserId();
            await _permissionService.DeleteAsync(adminId, id);
            return Ok(new { message = "Xóa permission thành công" });
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

    [HttpGet("role/{roleId:int}")]
    public async Task<IActionResult> GetRolePermissions(int roleId)
    {
        try
        {
            var adminId = GetCurrentUserId();
            var dto = await _permissionService.GetRolePermissionsAsync(adminId, roleId);
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

    [HttpPost("role/{roleId:int}/assign")]
    public async Task<IActionResult> AssignPermissionsToRole(int roleId, [FromBody] AssignPermissionsToRoleRequestDto request)
    {
        if (request is null) return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var adminId = GetCurrentUserId();
            var dto = await _permissionService.AssignPermissionsToRoleAsync(adminId, roleId, request);
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
}

