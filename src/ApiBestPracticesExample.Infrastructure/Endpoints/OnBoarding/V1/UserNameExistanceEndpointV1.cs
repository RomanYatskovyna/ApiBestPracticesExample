namespace ApiBestPracticesExample.Infrastructure.Endpoints.OnBoarding.V1;
public sealed class UserNameExistenceEndpointV1 : Endpoint<string, bool>
{
	private readonly AppDbContext _context;

	public override void Configure()
	{
		Post("onboarding/validate-user-name-uniqueness");
		AllowAnonymous();
		Description(d => { d.WithDisplayName("ValidateUserName"); });
		Summary(s =>
		{
			s.Summary = "short summary goes here";
			s.Description = "long description goes here";
		});
		Version((int)ApiSupportedVersions.V1);
	}

	public UserNameExistenceEndpointV1(AppDbContext context)
	{
		_context = context;
	}

	public override async Task HandleAsync(string req, CancellationToken ct)
	{
		Response = await _context.Users.AnyAsync(u => u.UserName == req, cancellationToken: ct);
	}
}
