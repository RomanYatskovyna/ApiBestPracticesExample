using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;
public class GoogleCallbackEndpoint : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get("authentication/signin-google-callback");
		AllowAnonymous();
		Version((int)ApiSupportedVersions.V1);
	}

	public override async Task HandleAsync(CancellationToken ct)
	{
		var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		if (!result.Succeeded)
		{
			// Handle error, the user might not have granted consent
			HttpContext.Response.Redirect("/error");
			return;
		}

		// Here you would typically find or create a user in your user store
		// For example:
		var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
		// var user = await FindOrCreateUser(email);

		// Issue the cookie for the signed-in user
		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Principal);

		HttpContext.Response.Redirect("/"); // Redirect to home page or user dashboard
	}
}