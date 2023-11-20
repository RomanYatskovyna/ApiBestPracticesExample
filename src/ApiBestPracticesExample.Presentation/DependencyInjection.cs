using ApiBestPracticesExample.Infrastructure.Caching;
using ApiBestPracticesExample.Infrastructure.Database;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using StackExchange.Redis;
using System.Reflection;
using Order = FastEndpoints.Order;

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
			config.IncludeAbstractValidators = true;
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
		services.AddJWTBearerAuth(configuration.GetRequiredSection("Authentication:Jwt").GetRequiredValue("AccessTokenSigningKey"), JWTBearer.TokenSigningStyle.Symmetric, v =>
		{
			v.ClockSkew = TimeSpan.FromSeconds(30);
			v.ValidateLifetime = true;
			v.ValidateIssuerSigningKey = true;
		});

		services.AddRedisOutputCache(configuration.GetConnectionString("RedisConnection"));
		return services;
	}
	public static WebApplication UseDefaultFastEndpoints(this WebApplication app)
	{
		app.UseFastEndpoints(config =>
		{
			config.Endpoints.Configurator = ep =>
			{
				ep.PostProcessors(Order.After, new ErrorLogger());
			};
			config.Endpoints.ShortNames = true;
			config.Endpoints.RoutePrefix = "api";
			config.Versioning.Prefix = "v";
			config.Versioning.PrependToRoute = true;
		});
		return app;
	}
	public static IServiceCollection AddCustomDbContextPool<TContext>(this IServiceCollection services, string connectionString, bool isDevelopment) where TContext : DbContext
	{
		return services.AddDbContextPool<TContext>(options =>
		{
			options.UseSqlServer(connectionString, sqlServerOptions =>
				sqlServerOptions.MigrationsAssembly("ApiBestPracticesExample.Infrastructure").UseDateOnlyTimeOnly())
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
			if (isDevelopment)
			{
				options.EnableDetailedErrors()
					.EnableSensitiveDataLogging();
			}
		});
	}

	public static async Task<IServiceProvider> PrepareDbAsync(this IServiceProvider services, bool migrateDatabase = true, bool initData = true, bool initDevelopmentData = true)
	{
		await using var scope = services.CreateAsyncScope();
		await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var logger = services.GetRequiredService<ILogger>();
		if (migrateDatabase)
		{
			await context.Database.MigrateAsync();
		}
		if (initData)
		{
			await context.SeedDefaultDataAsync();
			if (initDevelopmentData)
			{
				await context.SeedDevelopmentTestDataAsync();
			}
			logger.Information("Data seeded successfully");
		}
		return services;
	}
	public static IServiceCollection AddRedisOutputCache(this IServiceCollection services, string? redisConStr)
	{
		services.AddOutputCache();
		if (string.IsNullOrEmpty(redisConStr))
			return services;
		services.RemoveAll<IOutputCacheStore>();
		services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();
		services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConStr));
		return services;
	}
}
