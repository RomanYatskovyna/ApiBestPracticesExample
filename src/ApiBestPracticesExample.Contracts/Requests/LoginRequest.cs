using System.ComponentModel;

namespace ApiBestPracticesExample.Contracts.Requests;

public sealed class LoginRequest
{
    [DefaultValue("email")] public string Email { get; set; } = null!;

    [DefaultValue("Qwerty123$")] public string Password { get; set; } = null!;
}