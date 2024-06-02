using ApiBestPracticesExample.Contracts;
using ApiBestPracticesExample.Contracts.Dtos.Onboarding;
using ApiBestPracticesExample.Infrastructure.Mappers;

namespace ApiBestPracticesExample.Presentation.Endpoints.OnBoarding.V1;

public sealed class RegisterUserEndpointV1 : Endpoint<UserCreateDto, UserDto>
{
    private readonly AppDbContext _context;

    public RegisterUserEndpointV1(AppDbContext context)
    {
        _context = context;
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

    public override async Task HandleAsync(UserCreateDto req, CancellationToken ct)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == req.UserName, ct))
        {
            AddError(r => r.UserName, "User with this userName already exists");
        }

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
        Response = user.MapUserToUserDto();
    }
}