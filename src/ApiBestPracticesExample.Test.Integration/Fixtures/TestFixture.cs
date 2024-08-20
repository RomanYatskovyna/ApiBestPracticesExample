using ApiBestPracticesExample.Presentation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Respawn.Graph;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;

public sealed class TestFixture : AppFixture<IApiMarker>
{
    private readonly ConnectionProviderBase _connectionProvider;

    private Respawner _respawner = null!;

    public TestFixture(IMessageSink s) : base(s)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json")
            .Build();

        var conStr = configuration.GetConnectionString("SqlConnection");

        _connectionProvider = string.IsNullOrEmpty(conStr)
            ? new DockerConnectionProvider()
            : new ExternalConnectionProvider(conStr);
    }

    protected override async Task PreSetupAsync()
    {
        await _connectionProvider.InitializeAsync();
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        s.RemoveAll(typeof(DbContextOptions<AppDbContext>));
        s.RemoveAll(typeof(AppDbContext));
        var conStr = _connectionProvider.GetDbConnectionString();
        s.AddCustomDbContextPool<AppDbContext>(conStr, true);

        base.ConfigureServices(s);
    }

    protected override async Task SetupAsync()
    {
        var conStr = _connectionProvider.GetDbConnectionString();

        if (!await CanConnectAsync(conStr))
        {
            return;
        }

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

    public async Task InitDatabaseAsync()
    {
        await Services.PrepareDbAsync();

    }

    protected override async Task TearDownAsync()
    {
        await _connectionProvider.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        var conStr = _connectionProvider.GetDbConnectionString();

        if (await CanConnectAsync(conStr))
        {
            await _respawner.ResetAsync(conStr);
        }
    }
    private static async Task<bool> CanConnectAsync(string connectionString)
    {
        try
        {
            await using SqlConnection connection = new SqlConnection(connectionString);

            await connection.OpenAsync();
            await connection.CloseAsync();
            
            return true;
        }
        catch (SqlException)
        {
            return false;
        }
    }
}