using Microsoft.Extensions.DependencyInjection;

namespace ApiBestPracticesExample.Test.Integration.InitDataHelpers;

public static class TestInitDataHelpers
{
    public static Task InitializeTestDataAsync(this IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();

        return Task.CompletedTask;
    }
}