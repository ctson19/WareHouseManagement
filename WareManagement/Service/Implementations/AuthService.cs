using BCrypt.Net;
using WareManagement.DTO.Authen;
using WareManagement.DTO.AuthenDTO;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Interfaces;
using WareManagement.Service;

namespace WareManagement.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly WareManagementContext _context;
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public AuthService(
            WareManagementContext context,
            IUserRepository userRepository,
            JwtService jwtService)
        {
            _context = context;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ipAddress)
        {
            if (request is null) return null;
            if (string.IsNullOrWhiteSpace(request.UserName)) return null;
            if (string.IsNullOrWhiteSpace(request.Password)) return null;

            var user = await _userRepository.GetByUsername(request.UserName);

            // PasswordHash được lưu dạng bcrypt (tương thích package BCrypt.Net-Next)
            var isSuccess = user is not null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            _context.UserLoginLogs.Add(new UserLoginLog
            {
                UserId = user?.Id,
                LoginTime = DateTime.UtcNow,
                IsSuccess = isSuccess,
                Ipaddress = ipAddress
            });
            await _context.SaveChangesAsync();

            if (!isSuccess || user is null) return null;

            return new LoginResponseDto
            {
                Token = _jwtService.GenerateToken(user),
                Username = user.Username
            };
        }

        public Task LogoutAsync(string? username, string? ipAddress)
        {
            // JWT stateless: để logout thật sự cần cơ chế refresh-token hoặc blacklist token.
            // Hiện tại endpoint chỉ giúp client xóa token.
            return Task.CompletedTask;
        }
    }
}
