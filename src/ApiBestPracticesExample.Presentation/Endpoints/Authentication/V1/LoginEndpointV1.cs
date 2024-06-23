using ApiBestPracticesExample.Contracts.Requests;
using ApiBestPracticesExample.Infrastructure.Services;
using FastEndpoints.Security;
using System.Security.Claims;

namespace ApiBestPracticesExample.Presentation.Endpoints.Authentication.V1;

public sealed class LoginEndpointV1 : Endpoint<LoginRequest, TokenResponse>
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public LoginEndpointV1(AppDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
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
        Version((int)ApiSupportedVersions.V1);
    }

    public override async Task<TokenResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user is null || !PasswordEncrypter.VerifyPassword(req.Password, user.PasswordHash))
        {
            ThrowError("Invalid user credentials", StatusCodes.Status404NotFound);
        }

        var tokenResponse = await CreateTokenWith<RefreshTokenEndpointV1>(user.Email, u =>
        {
            u.Roles.Add(user.RoleName);
            u.Claims.Add(new Claim(ClaimTypes.Email, user.Email));
            u.Claims.Add(new Claim(ClaimTypes.Name, user.Email));
        });

        _logger.Information("User {UserEmail} received token", user.Email);
        return tokenResponse;
    }
}