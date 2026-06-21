namespace Pinger.Application.DTOs.UserDTOs;

public class AllUserDetailResponseDto
{
    public ICollection<UserDetailResponseDto>  UserDetails { get; set; } =  new List<UserDetailResponseDto>();
}