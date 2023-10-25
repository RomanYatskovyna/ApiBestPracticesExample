using System.ComponentModel;

namespace ApiBestPracticesExample.Contracts.Requests;

public class LoginRequest
{
	[DefaultValue("1")]
	public string Username { get; set; } = null!;
	[DefaultValue("Qwerty123$")]
	public string Password { get; set; } = null!;
}