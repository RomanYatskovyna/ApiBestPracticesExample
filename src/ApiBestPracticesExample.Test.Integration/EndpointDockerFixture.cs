using ApiBestPracticesExample.Presentation;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

namespace ApiBestPracticesExample.Test.Integration;

public sealed class EndpointDockerFixture : DockerFixtureBase<IApiMarker>
{
	private const string DockerImage = "mcr.microsoft.com/mssql/server:2022-latest";
	private const int ContainerPort = 63000;
	public EndpointDockerFixture(IMessageSink messageSink) : base(messageSink, new Dictionary<string, DockerContainer>
	{
		{
			nameof(MsSqlContainer), new MsSqlBuilder()
				.WithImage(DockerImage)
				.WithName("TestDatabase-" + ContainerPort)
				.WithPassword("Qwerty123$")
				.WithPortBinding(ContainerPort.ToString(), "1433")
				.Build()
		}
	})
	{


	}
	protected override void ConfigureServices(IServiceCollection services)
	{
		services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
		services.RemoveAll(typeof(AppDbContext));
		var sqlContainer = (MsSqlContainer)DockerContainers[nameof(MsSqlContainer)] ;
		services.AddCustomDbContext<AppDbContext>(sqlContainer.GetConnectionString());

	}


}