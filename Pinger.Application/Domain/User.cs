namespace Pinger.Application.Domain;

public class User: BaseEntity
{
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    
    public ICollection<UserRole>  UserRoles { get; set; } =  new List<UserRole>();
}