using System.Reflection;
using ApiBestPracticesExample.Infrastructure.Caching;
using FastEndpoints.Swagger;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ApiBestPracticesExample.Infrastructure.Database;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using Order = FastEndpoints.Order;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ApiBestPracticesExample.Presentation;

public static class DependencyInjection
{
	public static IServiceCollection AddDefaultServices(this IServiceCollection services, IConfiguration configuration, List<Assembly> endpointAssemblies, List<Assembly> validationAssemblies, List<int> supportedVersions)
	{
		services.AddSerilog(logger =>
		{
			logger.ReadFrom.Configuration(configuration);
		});
		services.AddFastEndpoints(config =>
		{
			config.Assemblies = endpointAssemblies;
		});
		services.AddValidatorsFromAssemblies(validationAssemblies);
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

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		});
		services.AddAuthorization();
		services.AddJWTBearerAuth(configuration.GetRequiredSection("Jwt").GetRequiredValue("AccessTokenSigningKey"),JWTBearer.TokenSigningStyle.Symmetric,v=>
		{
			v.ClockSkew = TimeSpan.FromSeconds(30);
			v.ValidateLifetime = true;
			v.ValidateIssuerSigningKey = true;
		});

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
				ep.PostProcessors(Order.After,new ErrorLogger());
			};
			config.Endpoints.ShortNames = true;
			config.Endpoints.RoutePrefix = "api";
			config.Versioning.Prefix = "v";
			config.Versioning.PrependToRoute = true;
		});
		app.UseSwaggerGen();
		return app;
	}
	public static IServiceCollection AddCustomDbContext<TContext>(this IServiceCollection services, string connectionString,bool isDevelopment) where TContext : DbContext
	{
		return services.AddDbContext<TContext>(options =>
		{
			options.UseSqlServer(connectionString, sqlServerOptions =>
				sqlServerOptions.MigrationsAssembly("ApiBestPracticesExample.Infrastructure"))
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
			if (isDevelopment)
			{
				options.EnableDetailedErrors().EnableSensitiveDataLogging();
			}
		});
	}

	public static async Task<IServiceProvider> PerformDbPreparationAsync(this IServiceProvider services, bool initData = true, bool migrateDatabase = true)
	{
		await using var scope = services.CreateAsyncScope();
		await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var logger = services.GetRequiredService<ILogger>();
		if (migrateDatabase)
		{
			await context.Database.MigrateAsync();
			logger.Information("Database migrated successfully");
		}
		if (initData)
		{
			await context.SeedDataAsync(logger);
			logger.Information("Data seeded successfully");
		}
		return services;
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
