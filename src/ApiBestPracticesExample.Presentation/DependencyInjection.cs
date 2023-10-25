using System.Reflection;
using FastEndpoints.Swagger;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
			services.SwaggerDocument(swaggerConfig =>
			{
				swaggerConfig.MaxEndpointVersion = version;
				swaggerConfig.DocumentSettings = settings =>
				{
					settings.DocumentName = version.ToString();

					settings.Description = "123";
					settings.Title = "My API";
					settings.Version = version.ToString();
				};
				swaggerConfig.ShortSchemaNames = true;
			});


		services.AddAuthorization();
		services.AddJWTBearerAuth(configuration.GetRequiredSection("Jwt").GetRequiredValue("AccessTokenSigningKey"));
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
}
