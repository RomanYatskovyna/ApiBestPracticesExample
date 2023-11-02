using ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;
using ApiBestPracticesExample.Presentation;
using DotNet.Testcontainers.Containers;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;

public class EndpointDockerFixture : DockerCollectionFixtureBase<IApiMarker>
{
    private const int SqlContainerPort = 63000;
    private const int RedisContainerPort = 62000;
    public HttpClient Admin { get; private set; }
    public HttpClient Anonymous { get; private set; }

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
	protected override async Task SetupAsync()
	{
		Anonymous = Client;

		await SetupAdminDefaultClientAsync();
		await SetupClientDefaultClientAsync();
	}

	private async Task SetupAdminDefaultClientAsync()
	{
		var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new()
		{
			UserId = "ReadOnlyAdmin@gmail.com",
			Password = "Qwerty123$"
		});
		rsp.IsSuccessStatusCode.Should().BeTrue();
		Admin = CreateClient(c => c.DefaultRequestHeaders.Add(JwtBearerDefaults.AuthenticationScheme, res.AccessToken));
	}
	private async Task SetupClientDefaultClientAsync()
	{
		var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new()
		{
			UserId = "ReadOnlyClient@gmail.com",
			Password = "Qwerty123$"
		});
		rsp.IsSuccessStatusCode.Should().BeTrue();
		Client = CreateClient(c => c.DefaultRequestHeaders.Add(JwtBearerDefaults.AuthenticationScheme, res.AccessToken));
	}
	protected override void ConfigureServices(IServiceCollection services)
    {
        services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
        services.RemoveAll(typeof(AppDbContext));
        var sqlContainer = GetContainerByType<MsSqlContainer>();
        services.AddCustomDbContext<AppDbContext>(sqlContainer.GetConnectionString());

    }


}