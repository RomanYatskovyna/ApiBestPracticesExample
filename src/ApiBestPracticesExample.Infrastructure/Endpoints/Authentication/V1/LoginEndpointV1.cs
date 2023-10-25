using System.Security.Claims;
using ApiBestPracticesExample.Contracts.Requests;
using ApiBestPracticesExample.Infrastructure.Database;
using ApiBestPracticesExample.Infrastructure.Services;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;

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
			s.Summary = "short summary goes here";
			s.Description = "long description goes here";
		});
		Version((int)ApiSupportedVersions.V1);
	}

	public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
	{
		var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == req.Username, ct);
		if (user is null || !PasswordEncrypter.VerifyPassword(req.Password, user.PasswordHash))
			ThrowError("The supplied credentials are invalid!", 400);

		Response = await CreateTokenWith<TokenService>(user.Email, u =>
		{
			u.Roles.AddRange(new[] { user.RoleName });
			u.Claims.Add(new Claim("Email", user.Email));
		});
	}
}