using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;

public sealed class RefreshTokenEndpointV1 : RefreshTokenService<TokenRequest, TokenResponse>
{
	private readonly AppDbContext _context;
	public RefreshTokenEndpointV1(IConfiguration config, AppDbContext context)
	{
		_context = context;
		var jwtSection = config.GetRequiredSection("Authentication:Jwt");
		var signingKey = jwtSection["AccessTokenSigningKey"];
		if (signingKey is null)
			throw new ArgumentNullException("AccessTokenSigningKey is null");
		if (signingKey.Length < 128)
			throw new ArgumentOutOfRangeException(paramName: "AccessTokenSigningKey must be more than 128 symbols");
		Setup(o =>
		{
			o.TokenSigningKey = jwtSection["AccessTokenSigningKey"];
			o.AccessTokenValidity = TimeSpan.FromMinutes(jwtSection.GetValue<int>("AccessTokenExpirationInMinutes"));
			o.RefreshTokenValidity = TimeSpan.FromHours(jwtSection.GetValue<int>("RefreshTokenExpirationInHours"));
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
		var user = await _context.Users.AsTracking().SingleAsync(u => u.Email == response.UserId);
		user.RefreshToken = response.RefreshToken;
		user.RefreshTokenExpiration = response.RefreshExpiry;
		await _context.SaveChangesAsync();
	}
	public override async Task RefreshRequestValidationAsync(TokenRequest req)
	{
		var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == req.UserId);
		if(user is null)
			ThrowError(r => r.UserId, "User with this UserId does not exist",StatusCodes.Status404NotFound);
		if (user.RefreshToken is null || user.RefreshToken != req.RefreshToken)
			ThrowError(r => r.RefreshToken, "Refresh token is invalid!");
		if (user.RefreshTokenExpiration is null || user.RefreshTokenExpiration < DateTime.Now.AddHours(Config.GetValue<int>("RefreshTokenExpirationInHours")))
			ThrowError(r => r.RefreshToken, "Refresh token is expired!");
	}

	public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
	{
		var user = await _context.Users.SingleAsync(u => u.Email == request.UserId);
		privileges.Roles.Add(user.RoleName);
		privileges.Claims.Add(new Claim(ClaimTypes.Email, user.Email));
	}
}