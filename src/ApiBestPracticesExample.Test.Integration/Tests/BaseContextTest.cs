using Microsoft.Extensions.DependencyInjection;

namespace ApiBestPracticesExample.Test.Integration.Tests;
public abstract class BaseContextTest : TestClass<EndpointDockerFixture>
{
	protected readonly AppDbContext DbContext;

	protected BaseContextTest(EndpointDockerFixture f, ITestOutputHelper o) : base(f, o)
	{
		DbContext = fx.Services.GetRequiredService<AppDbContext>();
	}
}
