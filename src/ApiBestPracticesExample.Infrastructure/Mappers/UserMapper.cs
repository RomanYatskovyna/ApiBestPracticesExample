using ApiBestPracticesExample.Contracts.Dtos.Onboarding;
using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Services;
using Riok.Mapperly.Abstractions;

namespace ApiBestPracticesExample.Infrastructure.Mappers;
[Mapper]
public static partial class UserMapper
{
	[MapProperty(nameof(User.RoleName), nameof(UserDto.Role))]
	public static partial UserDto MapUserToUserDto(this User user);

	public static User MapUserCreateDtoToUser(this UserCreateDto dto)
	{
		var user = UserCreateDtoToUser(dto);
		user.PasswordHash = PasswordEncrypter.HashPassword(dto.Password);
		return user;
	}
	private static partial User UserCreateDtoToUser(UserCreateDto car);
}
