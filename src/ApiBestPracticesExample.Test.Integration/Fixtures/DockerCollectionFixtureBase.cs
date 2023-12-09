using Bogus;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;


public abstract class DockerCollectionFixtureBase<TProgram> : BaseFixture, IAsyncLifetime where TProgram : class
{
    private readonly IMessageSink _messageSink;
    private readonly Dictionary<string, DockerContainer> _dockerContainers;

    protected TContainer GetContainerByType<TContainer>() where TContainer : DockerContainer
    {
        return GetContainerByName<TContainer>(typeof(TContainer).Name);
    }
    protected TContainer GetContainerByName<TContainer>(string containerName) where TContainer : DockerContainer
    {
        return (TContainer)_dockerContainers[containerName];
    }
    protected DockerCollectionFixtureBase(IMessageSink messageSink, Dictionary<string, DockerContainer> dockerContainers)
    {
        _messageSink = messageSink;
        _dockerContainers = dockerContainers;
    }
    /// <inheritdoc/>>
    public Faker Fake => Faker;

    /// <summary>
    /// the service provider of the bootstrapped web application
    /// </summary>
    public IServiceProvider Services => _app.Services;

    /// <summary>
    /// the test server of the underlying <see cref="WebApplicationFactory{TEntryPoint}"/>
    /// </summary>
    public TestServer Server => _app.Server;

    /// <summary>
    /// the default http client
    /// </summary>
    public HttpClient Client { get; set; } = null!;

    private WebApplicationFactory<TProgram> _app = null!;

    /// <summary>
    /// override this method if you'd like to do some one-time setup for the test-class.
    /// it is run before any of the test-methods of the class is executed.
    /// </summary>
    protected virtual Task SetupAsync() => Task.CompletedTask;

    /// <summary>
    /// override this method if you'd like to do some one-time teardown for the test-class.
    /// it is run after all test-methods have executed.
    /// </summary>
    protected virtual Task TearDownAsync() => Task.CompletedTask;

    /// <summary>
    /// override this method if you'd like to provide any configuration for the web host of the underlying <see cref="WebApplicationFactory{TEntryPoint}"/>/>
    /// </summary>
    protected virtual void ConfigureApp(IWebHostBuilder a) { }

    /// <summary>
    /// override this method if you'd like to override (remove/replace) any services registered in the underlying web application's DI container.
    /// </summary>
    protected virtual void ConfigureServices(IServiceCollection s) { }

    /// <summary>
    /// create a client for the underlying web application
    /// </summary>
    /// <param name="o">optional client options for the WAF</param>
    public HttpClient CreateClient(WebApplicationFactoryClientOptions? o = null) => CreateClient(_ => { }, o);

    /// <summary>
    /// create a client for the underlying web application
    /// </summary>
    /// <param name="c">configuration action for the client</param>
    /// <param name="o">optional client options for the WAF</param>
    public HttpClient CreateClient(Action<HttpClient> c, WebApplicationFactoryClientOptions? o = null)
    {
        var client = o is null ? _app.CreateClient() : _app.CreateClient(o);
        c(client);
        return client;
    }

    /// <summary>
    /// create a http message handler for the underlying web host/test server
    /// </summary>
    public HttpMessageHandler CreateHandler(Action<HttpContext>? c = null)
        => c is null ? _app.Server.CreateHandler() : _app.Server.CreateHandler(c);

    public async Task InitializeAsync()
    {
        var dockerTasks = _dockerContainers.Select(container => container.Value.StartAsync()).ToArray();
        await Task.WhenAll(dockerTasks);
        _app = (WebApplicationFactory<TProgram>)
            AppCache.GetOrAdd(
                GetType(),
                _ => new WebApplicationFactory<TProgram>().WithWebHostBuilder(b =>
                {
                    b.UseEnvironment("Testing");
                    b.ConfigureLogging(l => l.ClearProviders().AddXUnit(_messageSink));
                    b.ConfigureTestServices(ConfigureServices);
                    ConfigureApp(b);
                }));
        Client = _app.CreateClient();
        await SetupAsync();
    }

    public virtual async Task DisposeAsync()
    {
        await TearDownAsync();
        var dockerTasks = _dockerContainers.Select(container => container.Value.StopAsync()).ToArray();
        await Task.WhenAll(dockerTasks);
    }
}
