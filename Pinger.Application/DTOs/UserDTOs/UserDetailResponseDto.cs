namespace Pinger.Application.DTOs.UserDTOs;

public class UserDetailResponseDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    public IEnumerable<UserRoleResponseDto> UserRoles { get; set; } =  new List<UserRoleResponseDto>();
}