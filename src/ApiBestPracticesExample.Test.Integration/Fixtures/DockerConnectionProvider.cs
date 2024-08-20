using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;
public sealed class DockerConnectionProvider : ConnectionProviderBase
{

    private const int SqlContainerPort = 63000;
    private const int RedisContainerPort = 62000;

    private readonly RedisContainer _redisContainer;

    private readonly MsSqlContainer _sqlContainer;

    public DockerConnectionProvider(bool enablePermanentPort = false)
    {
        var sqlBuilder = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Qwerty123$");
        var redisBuilder = new RedisBuilder()
            .WithImage("redis:latest");

        if (enablePermanentPort)
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
    public override Task InitializeAsync()
    {
        var tasks = new[]
        {
            _sqlContainer.StartAsync(),
            _redisContainer.StartAsync(),
        };

        return Task.WhenAll(tasks);
    }

    public override string GetDbConnectionString() => _sqlContainer.GetConnectionString();
    public override Task DisposeAsync()
    {
        var tasks = new[] { _sqlContainer.StopAsync(), _redisContainer.StopAsync() };

        return Task.WhenAll(tasks);
    }
}
