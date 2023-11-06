using System.ComponentModel;

namespace ApiBestPracticesExample.Contracts.Dtos.User;
/// <summary>
/// the admin login request summary
/// </summary>
public class UserCreateDto
{
	/// <summary>
	/// User email
	/// </summary>
	[DefaultValue("Test@gmail.com")] 
	public string Email { get; set; } = null!;
	/// <summary>
	/// User password
	/// </summary>
	[DefaultValue("Qwerty123$")]
	public string Password { get; set; } = null!;

}