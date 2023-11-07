using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;
public sealed class SignInWithGoogleEndpoint : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get("authentication/signin-google");
		AllowAnonymous();
	}

	public override Task HandleAsync(CancellationToken ct)
	{
		var authenticationProperties = new AuthenticationProperties
		{
			RedirectUri = "authentication/signin-google-callback" // The URL to redirect to after authentication
		};
		return HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, authenticationProperties);
	}
}