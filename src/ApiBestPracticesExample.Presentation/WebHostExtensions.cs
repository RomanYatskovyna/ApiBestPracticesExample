using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace ApiBestPracticesExample.Presentation;

public static class IWebHostExtensions
{
	public static bool IsInKubernetes(this IServiceProvider services)
	{
		var cfg = services.GetService<IConfiguration>();
		var orchestratorType = cfg.GetValue<string>("OrchestratorType");
		return orchestratorType?.ToUpper() == "K8S";
	}

	public static async Task<IServiceProvider> MigrateDbContextAsync<TContext>(this IServiceProvider services, Action<IServiceProvider> seeder) where TContext : DbContext
	{
		var underK8S = services.IsInKubernetes();

		await using var scope = services.CreateAsyncScope();
		var scopeServices = scope.ServiceProvider;
		var logger = scopeServices.GetRequiredService<ILogger>();
		await using var context = scopeServices.GetRequiredService<TContext>();
		try
		{
			logger.Error("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

			if (underK8S)
			{
				await InvokeSeederAsync(seeder, context, scopeServices);
			}
			else
			{
				var retries = 10;
				var retry = Policy.Handle<SqlException>()
					.WaitAndRetry(
						retryCount: retries,
						sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
						onRetry: (exception, timeSpan, retry, ctx) =>
						{
							logger.Warning(exception, "[{Prefix}] Error migrating database (attempt {Retry} of {Retries})", nameof(TContext), retry, retries);
						});

				//if the sql server container is not created on run docker compose this
				//migration can't fail for network related exception. The retry options for DbContext only 
				//apply to transient exceptions
				// Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
				await retry.Execute(() => InvokeSeederAsync(seeder, context, scopeServices));
			}

			logger.Information("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
		}
		catch (Exception ex)
		{
			logger.Error(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
			if (underK8S)
			{
				throw;          // Rethrow under k8s because we rely on k8s to re-run the pod
			}
		}

		return services;
	}

	private static async Task InvokeSeederAsync<TContext>(Action<IServiceProvider> seeder, TContext context, IServiceProvider services)
		where TContext : DbContext
	{
		await context.Database.MigrateAsync();
		seeder(services);
	}
}
