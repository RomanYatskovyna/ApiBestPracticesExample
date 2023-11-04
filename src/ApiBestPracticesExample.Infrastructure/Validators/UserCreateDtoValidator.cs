using ApiBestPracticesExample.Contracts.Dtos.User;
using FluentValidation;

namespace ApiBestPracticesExample.Infrastructure.Validators;

public sealed class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
	public UserCreateDtoValidator()
	{
		RuleFor(x => x.Email)
			.EmailAddress()
			.WithMessage("Not valid email address")
			.MaximumLength(100)
			.WithMessage("Email length must be less then 100 symbols");

		RuleFor(x => x.Password)
			.MinimumLength(8)
			.WithMessage("Password length must be greater then 8 symbols")
			.MaximumLength(64)
			.WithMessage("Password length must be less then 64 symbols");

	}
}