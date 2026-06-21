namespace Pinger.Application.DTOs.UserDTOs;

public class UserRoleResponseDto
{
    public int RoleId { get; set; }
    public required string RoleName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}