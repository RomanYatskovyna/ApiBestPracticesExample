using ApiBestPracticesExample.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;

public sealed class RefreshTokenEndpointV1 : RefreshTokenService<TokenRequest, TokenResponse>
{
	private readonly IConfiguration _config;
	private readonly AppDbContext _context;
	public RefreshTokenEndpointV1(IConfiguration config, AppDbContext context)
	{
		_config = config;
		_context = context;
		var jwtSection = config.GetRequiredSection("Jwt");
		var signingKey = jwtSection["AccessTokenSigningKey"];
		if (signingKey is null)
			throw new ArgumentNullException("AccessTokenSigningKey is null");
		if (signingKey.Length < 128)
			throw new ArgumentOutOfRangeException("AccessTokenSigningKey must be more than 128 symbols");
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
		var user = await _context.Users.AsTracking().SingleAsync(u => u.Email == req.UserId);
		if (user.RefreshToken is null || user.RefreshToken != req.RefreshToken)
			AddError(r => r.RefreshToken, "Refresh token is invalid!");
		if (user.RefreshTokenExpiration is null || user.RefreshTokenExpiration < DateTime.Now.AddHours(Config.GetValue<int>("RefreshTokenExpirationInHours")))
			AddError(r => r.RefreshToken, "Refresh token is expired!");
	}

	public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
	{
		//privileges.Roles.Add("Manager");
		//privileges.Claims.Add(new Claim("ManagerID", request.UserId));
		//privileges.Permissions.Add("Manage_Department");

		// specify the user privileges to be embedded in the jwt when a refresh request is
		// received and validation has passed. this only applies to renewal/refresh requests
		// received to the refresh endpoint and not the initial jwt creation.        
	}
}