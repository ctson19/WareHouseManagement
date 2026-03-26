using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WareManagement.DTO.Authen;
using WareManagement.DTO.AuthenDTO;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.LoginAsync(request, ipAddress);

            if (result is null)
                return Unauthorized(new { message = "Sai thông tin tài khoản hoặc mật khẩu" });

            return Ok(result);
        }

        // JWT stateless: logout thật sự chỉ có nghĩa là client xóa token.
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _authService.LogoutAsync(User.Identity?.Name, ipAddress);
            return Ok(new { message = "Logout thành công" });
        }
    }
}
