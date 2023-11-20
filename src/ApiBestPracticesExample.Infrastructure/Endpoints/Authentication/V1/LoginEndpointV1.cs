using ApiBestPracticesExample.Infrastructure.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

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
		//Options(x => x.CacheOutput(p =>
		//{
		//	p.Tag("login");
		//	p.Expire(TimeSpan.FromSeconds(60));
		//}));
		Version((int)ApiSupportedVersions.V1);
	}

	public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
	{
		var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == req.UserId, ct);
		if (user is null || !PasswordEncrypter.VerifyPassword(req.Password, user.PasswordHash))
			ThrowError("User with supplied credentials was not found", StatusCodes.Status404NotFound);
		Response = await CreateTokenWith<RefreshTokenEndpointV1>(user.Email, u =>
		{
			u.Roles.AddRange(new[] { user.RoleName });
			u.Claims.Add(new Claim(ClaimTypes.Email,user.Email));
		});
	}
}