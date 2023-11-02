using Microsoft.Extensions.DependencyInjection;

namespace ApiBestPracticesExample.Test.Integration.Tests;
[Collection("DockerCollection")]
public abstract class BaseTest
{
	protected readonly EndpointDockerFixture Fixture;
	protected readonly ITestOutputHelper OutputHelper;
	protected readonly AppDbContext DbContext;

	protected BaseTest(EndpointDockerFixture fixture, ITestOutputHelper outputHelper)
	{
		Fixture = fixture;
		OutputHelper = outputHelper;
		DbContext = Fixture.Services.GetRequiredService<AppDbContext>();
	}

	protected void LoginAsDefaultAdmin()
	{

	}
}
