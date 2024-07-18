using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;

namespace ApiBestPracticesExample.Test.Integration.Fixtures;

public abstract class ConnectionProviderBase : IAsyncLifetime
{
    public abstract string GetDbConnectionString();

    public virtual Task InitializeAsync()
    {

        return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}