namespace ApiBestPracticesExample.Presentation.Endpoints.V1.OnBoarding;

public sealed class EmailExistenceEndpointV1 : Endpoint<string, bool>
{
    private readonly AppDbContext _context;

    public EmailExistenceEndpointV1(AppDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("onboarding/validate-email-uniqueness");
        AllowAnonymous();
        Description(d => { d.WithDisplayName("ValidateEmail"); });
        Summary(s =>
        {
            s.Summary = "Checks if user with such email already exists";
        });
        Version((int)ApiSupportedVersions.V1);
    }

    public override Task<bool> ExecuteAsync(string req, CancellationToken ct)
    {
        return _context.Users.AnyAsync(u => u.Email == req, ct);
    }
}