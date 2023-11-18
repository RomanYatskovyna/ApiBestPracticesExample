using System;

namespace ApiBestPracticesExample.Domain.Entities;

public partial class User
{
    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiration { get; set; }

    public virtual Role RoleNameNavigation { get; set; } = null!;
}
