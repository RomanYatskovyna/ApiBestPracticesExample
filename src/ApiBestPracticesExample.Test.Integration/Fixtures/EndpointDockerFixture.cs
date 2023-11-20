using ApiBestPracticesExample.Presentation;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;

public class EndpointDockerFixture(IMessageSink messageSink) : DockerCollectionFixtureBase<IApiMarker>(messageSink, new Dictionary<string, DockerContainer>
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
	private const int SqlContainerPort = 63000;
	private const int RedisContainerPort = 62000;

	private Respawner _respawner = null!;

	public Task InitDatabaseAsync()
	{
		return Services.PrepareDbAsync();
	}
	public async Task ResetDatabaseAsync()
	{
		var conStr = GetContainerByType<MsSqlContainer>().GetConnectionString();
		await _respawner.ResetAsync(conStr);
		var context = Services.GetRequiredService<AppDbContext>();
		context.ChangeTracker.Clear();
		AppDbContextSeeder.DefaultUser.Id = 0;
		AppDbContextSeeder.DefaultAdmin.Id = 0;
		AppDbContextSeeder.DefaultAdmin.RoleNameNavigation = null!;
		AppDbContextSeeder.DefaultUser.RoleNameNavigation = null!;
	}
	protected override async Task SetupAsync()
	{
		var sqlContainer = GetContainerByType<MsSqlContainer>();
		_respawner = await Respawner.CreateAsync(sqlContainer.GetConnectionString(), new RespawnerOptions
		{
			SchemasToInclude = new[]
			{
				"dbo"
			},
			TablesToIgnore = new[]
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