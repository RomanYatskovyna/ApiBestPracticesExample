using ApiBestPracticesExample.Contracts;
using ApiBestPracticesExample.Contracts.Dtos.Onboarding;
using ApiBestPracticesExample.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiBestPracticesExample.Presentation.Endpoints.OnBoarding.V1;

public sealed class RegisterUserEndpointV1 : Endpoint<UserCreateDto, Results<Ok<UserDto>, BadRequest<ProblemDetails>>>
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public RegisterUserEndpointV1(AppDbContext context,ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("onboarding/register");
        AllowAnonymous();
        Description(d => { d.WithDisplayName("RegisterUser"); });
        Summary(s =>
        {
            s.Summary = "short summary goes here";
            s.Description = "long description goes here";
        });
        Version((int)ApiSupportedVersions.V1);
    }

    public override async Task<Results<Ok<UserDto>, BadRequest<ProblemDetails>>> ExecuteAsync(UserCreateDto req, CancellationToken ct)
    {
        if (await _context.Users.AnyAsync(u => u.Email == req.Email, ct))
        {
            AddError(r => r.Email, "User with this email already exists");
        }

        if (await _context.Users.AnyAsync(u => u.PhoneNumber == req.PhoneNumber, ct))
        {
            AddError(r => r.PhoneNumber, "User with this phoneNumber already exists");
        }

        ThrowIfAnyErrors();

        var user = req.MapUserCreateDtoToUser();
        user.RoleName = SupportedRoles.User;

        await _context.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);

        _logger.Information("User {UserEmail} created", user.Email);

        var userDto = user.MapUserToUserDto();
        return TypedResults.Ok(userDto);
    }

}