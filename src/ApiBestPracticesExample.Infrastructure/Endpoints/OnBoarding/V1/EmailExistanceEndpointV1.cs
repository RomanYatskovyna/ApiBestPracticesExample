namespace ApiBestPracticesExample.Infrastructure.Endpoints.OnBoarding.V1;
public sealed class EmailExistenceEndpointV1 : Endpoint<string, bool>
{
	private readonly AppDbContext _context;

	public override void Configure()
	{
		Post("onboarding/validate-email");
		Description(d => { d.WithDisplayName("ValidateEmail"); });
		Summary(s =>
		{
			s.Summary = "short summary goes here";
			s.Description = "long description goes here";
		});
		Version((int)ApiSupportedVersions.V1);
	}

	public EmailExistenceEndpointV1(AppDbContext context)
	{
		_context = context;
	}

	public override async Task HandleAsync(string req, CancellationToken ct)
	{
		Response = await _context.Users.AnyAsync(u => u.Email == req, cancellationToken: ct);
	}
}
