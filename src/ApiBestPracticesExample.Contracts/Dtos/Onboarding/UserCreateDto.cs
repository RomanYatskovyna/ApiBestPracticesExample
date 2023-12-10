using System.ComponentModel;

namespace ApiBestPracticesExample.Contracts.Dtos.Onboarding;
/// <summary>
/// The dto for user creation
/// </summary>
public class UserCreateDto
{
	/// <summary>
	/// User unique name
	/// </summary>
	[DefaultValue("UniqueName")]
	public string UserName { get; set; } = null!;
	/// <summary>
	/// User email
	/// </summary>
	[DefaultValue("email@domain.com")] 
	public string Email { get; set; } = null!;
	/// <summary>
	/// User phoneNumber
	/// </summary>
	[DefaultValue("+3538991569")]
	public string? PhoneNumber { get; set; }
	/// <summary>
	/// User password
	/// </summary>
	[DefaultValue("Qwerty123$")]
	public string Password { get; set; } = null!;

}