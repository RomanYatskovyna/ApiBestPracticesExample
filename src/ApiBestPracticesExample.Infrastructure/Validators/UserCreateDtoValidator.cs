using ApiBestPracticesExample.Contracts.Dtos.User;
using FluentValidation;

namespace ApiBestPracticesExample.Infrastructure.Validators;

public sealed class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
	public UserCreateDtoValidator()
	{
		RuleFor(x => x.Email)
			.MinimumLength(8);
	}
}