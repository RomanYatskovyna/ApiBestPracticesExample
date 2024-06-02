using ApiBestPracticesExample.Presentation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;

public class ApiFixture : TestFixture<IApiMarker>
{
    private const int SqlContainerPort = 63000;
    private const int RedisContainerPort = 62000;

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .WithName("TestRedisDatabase-" + RedisContainerPort)
        .WithPortBinding(RedisContainerPort.ToString(), "6379")
        .Build();

    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithName("TestSqlDatabase-" + SqlContainerPort)
        .WithPassword("Qwerty123$")
        .WithPortBinding(SqlContainerPort.ToString(), "1433")
        .Build();

    private Respawner _respawner = null!;

    public ApiFixture(IMessageSink s) : base(s)
    {
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        InitDockerContainers();

        s.RemoveAll(typeof(DbContextOptions<AppDbContext>));
        s.RemoveAll(typeof(AppDbContext));
        var conStr = _sqlContainer.GetConnectionString();
        s.AddCustomDbContextPool<AppDbContext>(conStr, true);

        base.ConfigureServices(s);
    }

    private void InitDockerContainers()
    {
        var dockerStartupTasks = new[]
        {
            Task.Run(() => _sqlContainer.StartAsync()), Task.Run(() => _redisContainer.StartAsync()),
        };
        Task.WaitAll(dockerStartupTasks);
    }

    public Task InitDatabaseAsync()
    {
        return Services.PrepareDbAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        var conStr = _sqlContainer.GetConnectionString();
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
        var conStr = _sqlContainer.GetConnectionString();
        _respawner = await Respawner.CreateAsync(conStr, new RespawnerOptions
        {
            SchemasToInclude =
            [
                "dbo",
            ],
            TablesToIgnore =
            [
                new Table("__EFMigrationsHistory"),
            ],
            DbAdapter = DbAdapter.SqlServer,
        });
    }
}