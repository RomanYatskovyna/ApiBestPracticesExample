using ApiBestPracticesExample.Infrastructure.Services;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;

namespace ApiBestPracticesExample.Presentation.Endpoints.Authentication.V1;

public class LoginEndpointV1 : Endpoint<LoginRequest, TokenResponse>
{
    private readonly AppDbContext _context;

    public LoginEndpointV1(AppDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("authentication/login");
        AllowAnonymous();
        Description(d =>
        {
            d.WithDisplayName("Login");
        });
        Summary(s =>
        {
            s.Summary = "User authorization endpoint";
            s.Description = "Here user can login and receive access and refresh tokens";
        });
        //Options(x => x.CacheOutput(p =>
        //{
        //	p.Tag("login");
        //	p.Expire(TimeSpan.FromSeconds(60));
        //}));
        Version((int)ApiSupportedVersions.V1);
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == req.Email, ct);
        if (user is null || !PasswordEncrypter.VerifyPassword(req.Password, user.PasswordHash))
        {
            ThrowError("User with supplied credentials was not found", StatusCodes.Status404NotFound);
        }

        Response = await CreateTokenWith<RefreshTokenEndpointV1>(user.Email, u =>
        {
            u.Roles.AddRange([user.RoleName]);
            u.Claims.Add(new Claim(ClaimTypes.Email, user.Email));
        });
    }
}