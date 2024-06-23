namespace ApiBestPracticesExample.Domain.Entities;

public partial class RefreshToken
{
    public string UserEmail { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public virtual User UserEmailNavigation { get; set; } = null!;
}