using FluentValidation;

namespace ApiBestPracticesExample.Contracts.Dtos.User;

public sealed class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
	public UserCreateDtoValidator()
	{
		RuleFor(x => x.Email)
			.EmailAddress().WithMessage("Email address is not valid")
			.MaximumLength(100).WithMessage("Email length must be less then 100 symbols");

		RuleFor(x => x.Password)
			.MinimumLength(8).WithMessage("Password must be at least 8 characters long")
			.MaximumLength(64).WithMessage("Password length must be less than 64 characters")
			.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
			.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
			.Matches("[0-9]").WithMessage("Password must contain at least one digit")
			.Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character")
			.Must(p => !p.Any(char.IsWhiteSpace)).WithMessage("Password must not contain spaces");

	}
}