using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Settings;
using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ApiBestPracticesExample.Presentation.Endpoints.Authentication.V1;

public sealed class RefreshTokenEndpointV1 : RefreshTokenService<TokenRequest, TokenResponse>
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public RefreshTokenEndpointV1(IOptions<JwtOptions> jwtOptions, AppDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;

        var jwtOptionsValue = jwtOptions.Value;
        Setup(o =>
        {
            o.TokenSigningKey = jwtOptionsValue.AccessTokenSigningKey;
            o.AccessTokenValidity = TimeSpan.FromMinutes(jwtOptionsValue.AccessTokenExpirationInMinutes);
            o.RefreshTokenValidity = TimeSpan.FromHours(jwtOptionsValue.RefreshTokenExpirationInHours);

            o.Endpoint("authentication/refresh-token", ep =>
            {
                ep.Description(d =>
                {
                    d.WithDisplayName("Login");
                });
                ep.Summary(s =>
                {
                    s.Summary = "this is the refresh token endpoint";
                    s.Description = "You can refresh your token here when the access token is expired";
                });
                Version((int)ApiSupportedVersions.V1);
            });
        });
    }

    public override async Task PersistTokenAsync(TokenResponse response)
    {
        var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.UserEmail == response.UserId);

        if (refreshToken is null)
        {
            var user = await _context.Users
                .AsTracking()
                .SingleAsync(u => u.Email == response.UserId);

            refreshToken = new RefreshToken
            {
                Token = response.RefreshToken, ExpiryDate = response.RefreshExpiry, UserEmailNavigation = user,
            };

            await _context.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            _logger.Information("Created new refresh token for user {UserEmail}", user.Email);
        }
        else
        {
            refreshToken.Token = response.RefreshToken;
            refreshToken.ExpiryDate = response.RefreshExpiry;

            _context.Update(refreshToken);
            await _context.SaveChangesAsync();

            _logger.Information("Updated refresh token for user {UserEmail}", refreshToken.UserEmail);
        }
    }

    public override async Task RefreshRequestValidationAsync(TokenRequest req)
    {
        var refreshToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(u => u.UserEmail == req.UserId);

        if (refreshToken is null)
        {
            ThrowError(r => r.UserId, "User with this UserId does not exist", StatusCodes.Status404NotFound);
        }

        if (refreshToken.Token != req.RefreshToken)
        {
            ThrowError(r => r.RefreshToken, "Refresh token is invalid!");
        }

        if (refreshToken.ExpiryDate <= DateTime.UtcNow)
        {
            ThrowError(r => r.RefreshToken, "Refresh token is expired!");
        }

        _logger.Information("Validated refresh token for user {UserEmail}", refreshToken.UserEmail);
    }

    public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
    {
        var user = await _context.Users.SingleAsync(u => u.Email == request.UserId);

        privileges.Roles.Add(user.RoleName);
        privileges.Claims.Add(new Claim(ClaimTypes.Email, user.Email));

        _logger.Information("Set privileges for user {UserEmail}", user.Email);
    }
}