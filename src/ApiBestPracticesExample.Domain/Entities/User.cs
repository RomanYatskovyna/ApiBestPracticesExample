namespace ApiBestPracticesExample.Domain.Entities;

public class User
{
    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public virtual RefreshToken? RefreshToken { get; set; }

    public virtual Role RoleNameNavigation { get; set; } = null!;
}