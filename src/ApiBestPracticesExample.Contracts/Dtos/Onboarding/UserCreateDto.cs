using FluentValidation;
using System.ComponentModel;

namespace ApiBestPracticesExample.Contracts.Dtos.Onboarding;

/// <summary>
///     The dto for user creation
/// </summary>
public sealed record UserCreateDto
{
    /// <summary>
    ///     User email
    /// </summary>
    [DefaultValue("email@domain.com")]
    public required string Email { get; init; }

    /// <summary>
    ///     User phoneNumber
    /// </summary>
    [DefaultValue("+3538991569")]
    public string? PhoneNumber { get; init; }

    /// <summary>
    ///     User password
    /// </summary>
    [DefaultValue("Qwerty123$")]
    public required string Password { get; init; }
}

public sealed class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email address is not valid")
            .MaximumLength(100).WithMessage("Email length must be less then 100 symbols");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(\+1)?(\s)?(\()?(\d{3})(?(3)\))([-.\s])?(\d{3})([-.\s])?(\d{4})$")
            .WithMessage("PhoneNumber address is not valid")
            ;

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