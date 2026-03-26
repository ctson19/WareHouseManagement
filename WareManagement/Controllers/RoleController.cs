using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WareManagement.DTO.RoleDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        private int GetCurrentUserId()
        {
            var idStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(idStr, out var userId))
                throw new InvalidOperationException("Id không hợp lệ");

            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var adminId = GetCurrentUserId();
                var roles = await _roleService.GetAllAsync(adminId);
                return Ok(roles);
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
                var role = await _roleService.GetByIdAsync(adminId, id);
                return Ok(role);
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
        public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto request)
        {
            if (request is null)
                return BadRequest(new { message = "Yêu cầu không được để trống" });

            try
            {
                var adminId = GetCurrentUserId();
                var created = await _roleService.CreateAsync(adminId, request);
                return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleRequestDto request)
        {
            if (request is null)
                return BadRequest(new { message = "Yêu cầu không được để trống" });

            try
            {
                var adminId = GetCurrentUserId();
                var updated = await _roleService.UpdateAsync(adminId, id, request);
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
                await _roleService.DeleteAsync(adminId, id);
                return Ok(new { message = "Xóa role thành công" });
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

        [HttpPost("{userId:int}/roles/assign")]
        public async Task<IActionResult> AssignRoles(int userId, [FromBody] AddRolesToUserRequestDto request)
        {
            if (request is null)
                return BadRequest(new { message = "Yêu cầu không được để trống" });

            try
            {
                var adminId = GetCurrentUserId();
                var result = await _roleService.AddRolesToUserAsync(adminId, userId, request);
                return Ok(result);
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
}

