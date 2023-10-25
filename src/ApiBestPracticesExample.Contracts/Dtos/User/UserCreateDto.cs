using System.ComponentModel;

namespace ApiBestPracticesExample.Contracts.Dtos.User;

public class UserCreateDto
{
	[DefaultValue("Test@gmail.com")] 
	public string Email { get; set; } = null!;
	[DefaultValue("Qwerty123$")]
	public string Password { get; set; } = null!;

}