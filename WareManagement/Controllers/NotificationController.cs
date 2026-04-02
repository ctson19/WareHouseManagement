using Microsoft.AspNetCore.Mvc;
using WareManagement.DTO.NotificationDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : BaseApiController
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? type = null,
        [FromQuery] bool? isRead = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetMyPagedAsync(userId, page, pageSize, type, isRead);
            return Ok(new { items = result.Items, total = result.Total, page, pageSize });
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
            var dto = await _notificationService.GetByIdAsync(userId, id);
            return Ok(dto);
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

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var dto = await _notificationService.MarkReadAsync(userId, id);
            return Ok(dto);
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

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _notificationService.MarkAllReadAsync(userId);
            return Ok(new { message = "Đã đánh dấu đã đọc", count });
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    [HttpPost("{userId:int}")]
    public async Task<IActionResult> CreateForUser(int userId, [FromBody] CreateNotificationRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var adminId = GetCurrentUserId();
            var created = await _notificationService.CreateForUserByAdminAsync(adminId, userId, request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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

