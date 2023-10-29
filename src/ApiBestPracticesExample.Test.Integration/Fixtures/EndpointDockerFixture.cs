using ApiBestPracticesExample.Presentation;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;

public class EndpointDockerFixture : DockerCollectionFixtureBase<IApiMarker>
{
    private const int SqlContainerPort = 63000;
    public EndpointDockerFixture(IMessageSink messageSink) : base(messageSink, new Dictionary<string, DockerContainer>
    {
        {
            nameof(MsSqlContainer), new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithName("TestDatabase-" + SqlContainerPort)
                .WithPassword("Qwerty123$")
                .WithPortBinding(SqlContainerPort.ToString(), "1433")
                .Build()
        }
    })
    {


    }
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
        services.RemoveAll(typeof(AppDbContext));
        var sqlContainer = GetContainerByType<MsSqlContainer>();
        services.AddCustomDbContext<AppDbContext>(sqlContainer.GetConnectionString());

    }


}