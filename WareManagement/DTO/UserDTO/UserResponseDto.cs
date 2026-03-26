namespace WareManagement.DTO.UserDTO;

public class UserResponseDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

