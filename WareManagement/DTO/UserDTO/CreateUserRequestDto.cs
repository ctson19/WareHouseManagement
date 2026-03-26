namespace WareManagement.DTO.UserDTO;

public class CreateUserRequestDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Mặc định tạo user đang hoạt động
    public bool IsActive { get; set; } = true;
}

