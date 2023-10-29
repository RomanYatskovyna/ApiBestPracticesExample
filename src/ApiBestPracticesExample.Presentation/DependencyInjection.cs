using System.Reflection;
using ApiBestPracticesExample.Infrastructure.Caching;
using FastEndpoints.Swagger;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ApiBestPracticesExample.Infrastructure.Database;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace ApiBestPracticesExample.Presentation;

public static class DependencyInjection
{
	public static IServiceCollection AddDefaultServices(this IServiceCollection services, IConfiguration configuration, List<Assembly> endpointAssemblies, List<int> supportedVersions)
	{

		services.AddSerilog(logger =>
		{
			logger.ReadFrom.Configuration(configuration);
		});
		services.AddFastEndpoints(config =>
		{
			config.Assemblies = endpointAssemblies;
		});
		foreach (var version in supportedVersions)
		{
			services.SwaggerDocument(swaggerConfig =>
			{
				swaggerConfig.MaxEndpointVersion = version;
				var swaggerSection = configuration.GetRequiredSection("Swagger");
				swaggerConfig.DocumentSettings = settings =>
				{
					settings.DocumentName = $"v{version}";
					settings.Title = swaggerSection["Title"];
					settings.Description = swaggerSection["Description"];
					settings.Version = version.ToString();
				};
				swaggerConfig.ShortSchemaNames = true;
			});
		}

		services.AddAuthorization();
		services.AddJWTBearerAuth(configuration.GetRequiredSection("Jwt").GetRequiredValue("AccessTokenSigningKey"));

		services.AddRedisOutputCache(configuration.GetRequiredConnectionString("RedisConnection"));
		return services;
	}
	public static WebApplication UseDefaultServices(this WebApplication app)
	{
		app.UseSerilogRequestLogging();
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseFastEndpoints(config =>
		{
			config.Endpoints.Configurator = ep =>
			{
				//ep.PostProcessors(new ErrorLogger());
			};
			config.Endpoints.ShortNames = true;
			config.Endpoints.RoutePrefix = "api";
			config.Versioning.Prefix = "v";
			config.Versioning.PrependToRoute = true;
		});
		app.UseSwaggerGen();
		return app;
	}
	public static IServiceCollection AddCustomDbContext<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
	{
		return services.AddDbContext<TContext>(options => options
					.UseSqlServer(connectionString, sqlServerOptions =>
						sqlServerOptions.MigrationsAssembly("ApiBestPracticesExample.Infrastructure"))
					.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
	}

	public static async Task<WebApplication> PerformDbPreparationAsync(this WebApplication app)
	{
		await using var scope = app.Services.CreateAsyncScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await context.Database.MigrateAsync();
		var logger = app.Services.GetRequiredService<ILogger>();
		logger.Information("Database migrated successfully");
		await context.SeedDataAsync(logger);
		return app;
	}

	public static IServiceCollection AddRedisOutputCache(this IServiceCollection services, string? redisConStr)
	{
		services.AddOutputCache();
		if (redisConStr is null)
			return services;
		services.RemoveAll<IOutputCacheStore>();
		services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();
		services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConStr));
		return services;
	}
}
