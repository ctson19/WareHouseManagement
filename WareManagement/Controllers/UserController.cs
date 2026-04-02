using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WareManagement.DTO.UserDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetCurrentUserId()
        {
            var idStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(idStr, out var userId))
                throw new InvalidOperationException("ID ng??i dùng trong JWT khùng h?p l?.");

            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
        {
            if (request is null)
                return BadRequest(new { message = "Yùu c?u khùng ???c ?? tr?ng." });

            try
            {
                var adminId = GetCurrentUserId();
                var created = await _userService.CreateUserAsync(adminId, request);
                return CreatedAtAction(nameof(CreateUser), new { id = created.Id }, created);
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
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePasswordForMe([FromBody] ChangePasswordRequestDto request)
        {
            if (request is null)
                return BadRequest(new { message = "Yùu c?u khùng ???c ?? tr?ng" });

            try
            {
                var userId = GetCurrentUserId();
                await _userService.ChangePasswordForMeAsync(userId, request);
                return Ok(new { message = "c?p nh?t m?t kh?u thùnh cùng" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var adminId = GetCurrentUserId();
                await _userService.SoftDeleteAsync(adminId, id);
                return Ok(new { message = "Xùa user thùnh cùng" });
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
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateUserStatusRequestDto request)
        {
            if (request is null)
                return BadRequest(new { message = "Yùu c?u khùng ???c ?? tr?ng" });

            try
            {
                var adminId = GetCurrentUserId();
                var updated = await _userService.UpdateUserStatusAsync(adminId, id, request.IsActive);
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
        }

        [HttpPut("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPasswordForUser(int id, [FromBody] ResetPasswordRequestDto request)
        {
            if (request is null)
                return BadRequest(new { message = "Yùu c?u khùng ???c ?? tr?ng" });

            try
            {
                var adminId = GetCurrentUserId();
                await _userService.ResetPasswordForUserAsync(adminId, id, request);
                return Ok(new { message = "M?t kh?u ???c t?o l?i thùnh cùng" });
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
        }
    }
}
