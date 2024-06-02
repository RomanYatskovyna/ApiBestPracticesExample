namespace ApiBestPracticesExample.Presentation.Endpoints.OnBoarding.V1;

public sealed class UserNameExistenceEndpointV1(AppDbContext context) : Endpoint<string, bool>
{
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

    public override async Task HandleAsync(string req, CancellationToken ct)
    {
        context = new AppDbContext(new DbContextOptions<AppDbContext>());
        Response = await context.Users.AnyAsync(u => u.UserName == req, ct);
    }
}