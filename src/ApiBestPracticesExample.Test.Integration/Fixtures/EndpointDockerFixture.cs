using ApiBestPracticesExample.Presentation;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;

public class EndpointDockerFixture : DockerCollectionFixtureBase<IApiMarker>
{
	private const int SqlContainerPort = 63000;
	private const int RedisContainerPort = 62000;

	private Respawner Respawner = null!;
	public EndpointDockerFixture(IMessageSink messageSink) : base(messageSink, new Dictionary<string, DockerContainer>
	{
		{
			nameof(MsSqlContainer), new MsSqlBuilder()
				.WithImage("mcr.microsoft.com/mssql/server:2022-latest")
				.WithName("TestSqlDatabase-" + SqlContainerPort)
				.WithPassword("Qwerty123$")
				.WithPortBinding(SqlContainerPort.ToString(), "1433")
				.Build()
		},
		{
			nameof(RedisContainer),  new RedisBuilder()
				.WithImage("redis:latest")
				.WithName("TestRedisDatabase-" + RedisContainerPort)
				.WithPortBinding(RedisContainerPort.ToString(), "6379")
				.Build()
		}
	})
	{
	}
	public Task InitDatabaseAsync()
	{
		return Services.PrepareDbAsync();
	}
	public Task ResetDatabaseAsync()
	{
		var sqlContainer = GetContainerByType<MsSqlContainer>();
		return Respawner.ResetAsync(sqlContainer.GetConnectionString());
	}
	protected override async Task SetupAsync()
	{
		var sqlContainer = GetContainerByType<MsSqlContainer>();
		Respawner = await Respawner.CreateAsync(sqlContainer.GetConnectionString(), new RespawnerOptions
		{
			SchemasToInclude = new[]
			{
				"dbo"
			},
			TablesToIgnore = new []
			{
				new Table("__EFMigrationsHistory")
			},
			DbAdapter = DbAdapter.SqlServer
		});
	}

	protected override void ConfigureServices(IServiceCollection services)
	{
		services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
		services.RemoveAll(typeof(AppDbContext));
		var sqlContainer = GetContainerByType<MsSqlContainer>();
		services.AddCustomDbContextPool<AppDbContext>(sqlContainer.GetConnectionString(), true);
	}


}