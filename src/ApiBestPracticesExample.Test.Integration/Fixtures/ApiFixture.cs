using ApiBestPracticesExample.Presentation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;

public class ApiFixture : AppFixture<IApiMarker>
{

    private readonly bool _enablePermanentPort = false;

    private const int SqlContainerPort = 63000;
    private const int RedisContainerPort = 62000;


    private readonly RedisContainer _redisContainer;

    private readonly MsSqlContainer _sqlContainer;

    private Respawner _respawner = null!;

    public ApiFixture(IMessageSink s) : base(s)
    {
        var sqlBuilder = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Qwerty123$");
        var redisBuilder = new RedisBuilder()
            .WithImage("redis:latest");
        if (_enablePermanentPort)
        {
            redisBuilder
                .WithName("TestRedisDatabase-" + RedisContainerPort)
                .WithPortBinding(RedisContainerPort.ToString(), "6379");
            sqlBuilder
                .WithName("TestSqlDatabase-" + SqlContainerPort)
                .WithPortBinding(SqlContainerPort.ToString(), "1433");
        }
        _redisContainer = redisBuilder.Build();
        _sqlContainer = sqlBuilder.Build();
    }

    protected override async Task PreSetupAsync()
    {
        await InitDockerContainersAsync();
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        s.RemoveAll(typeof(DbContextOptions<AppDbContext>));
        s.RemoveAll(typeof(AppDbContext));
        var conStr = _sqlContainer.GetConnectionString();
        s.AddCustomDbContextPool<AppDbContext>(conStr, true);

        base.ConfigureServices(s);
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

    private Task InitDockerContainersAsync()
    {
        var tasks = new[] { _sqlContainer.StartAsync(), _redisContainer.StartAsync() };

        return Task.WhenAll(tasks);
    }

    public Task InitDatabaseAsync()
    {
        return Services.PrepareDbAsync();
    }

    protected override async Task TearDownAsync()
    {
        var tasks = new[] { _sqlContainer.StopAsync(), _redisContainer.StopAsync() };

        await Task.WhenAll(tasks);
    }

    public async Task ResetDatabaseAsync()
    {
        var conStr = _sqlContainer.GetConnectionString();
        await _respawner.ResetAsync(conStr);

        var context = Services.GetRequiredService<AppDbContext>();
        context.ChangeTracker.Clear();
    }
}