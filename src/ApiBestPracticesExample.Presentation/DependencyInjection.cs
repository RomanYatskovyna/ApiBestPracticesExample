using ApiBestPracticesExample.Infrastructure.Caching;
using ApiBestPracticesExample.Infrastructure.Settings;
using FastEndpoints.ClientGen;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using System.Reflection;
using System.Text.Json.Serialization;
using Order = FastEndpoints.Order;

namespace ApiBestPracticesExample.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddDefaultServices(
        this IServiceCollection services,
        IConfiguration configuration,
        List<int> supportedVersions,
        params Assembly[] endpointAssemblies)
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

        services.AddAuthenticationJwtBearer(o =>
            {
                var tokenKey = configuration.GetRequiredValue("Authentication:Jwt:AccessTokenSigningKey");
                o.SigningKey = tokenKey;
                o.SigningStyle = TokenSigningStyle.Symmetric;

            }
           );
        return services;
    }

    public static WebApplication UseDefaultFastEndpoints(this WebApplication app)
    {
        app.UseFastEndpoints(config =>
        {
            config.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
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

    public static IServiceCollection AddDefaultOptions(this IServiceCollection services)
    {
        services.AddOptionsWithValidation<JwtOptions>("Authentication:Jwt");
        return services;
    }
    public static IServiceCollection AddCustomDbContextPool<TContext>(
        this IServiceCollection services,
        string connectionString,
        bool isDevelopment)
        where TContext : DbContext
    {
        return services.AddDbContextPool<TContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
                {
                    var assemblyName = typeof(AppDbContext).Assembly.FullName;
                    sqlServerOptions.MigrationsAssembly(assemblyName)
                        .UseDateOnlyTimeOnly();
                })
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            if (isDevelopment)
            {
                options.EnableDetailedErrors()
                    .EnableSensitiveDataLogging();
            }
        });
    }

    public static WebApplication UseClientGen(this WebApplication app, List<int> supportedVersions)
    {
        foreach (var version in supportedVersions)
        {
            var versionString = $"v{version}";
            app.MapCSharpClientEndpoint($"api/v{version}/cs-client", versionString, s =>
            {
                s.ClassName = $"ApiClient{versionString.ToUpper()}";
                s.CSharpGeneratorSettings.Namespace = "";
            });
            app.MapTypeScriptClientEndpoint($"api/v{version}/ts-client", versionString, s =>
            {
                s.ClassName = $"ApiClient{versionString.ToUpper()}";
                s.TypeScriptGeneratorSettings.Namespace = "Namespace";
            });
        }

        return app;
    }

    public static async Task<IServiceProvider> PrepareDbAsync(
        this IServiceProvider services,
        bool migrateDatabase = true,
        bool initData = true,
        bool initDevelopmentData = true
        )
    {
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (migrateDatabase)
        {
            await context.Database.MigrateAsync();
        }

        if (initData)
        {
            await context.SeedDefaultDataAsync();

            var logger = services.GetRequiredService<ILogger>();
            logger.Information("Default data seeded successfully");

            if (initDevelopmentData)
            {
                await context.SeedDevelopmentTestDataAsync();
                logger.Information("Development data seeded successfully");
            }
        }

        return services;
    }

    public static async Task<WebApplication> PrepareDbAsync(this WebApplication app)
    {

        var dbSection = app.Configuration.GetRequiredSection("Database");

        await app.Services.PrepareDbAsync(
            dbSection.GetRequiredValue<bool>("MigrateDatabase"),
            dbSection.GetRequiredValue<bool>("InitData"),
            !app.Environment.IsProduction()
        );

        return app;
    }

    public static IServiceCollection AddOutputCache(this IServiceCollection services, string? redisConStr)
    {
        services.AddOutputCache();

        if (string.IsNullOrEmpty(redisConStr))
        {
            return services;
        }

        services.RemoveAll<IOutputCacheStore>();
        services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConStr));

        return services;
    }

    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services,string sqlConStr, string? redisConStr)
    {
        var health = services.AddHealthChecks()
            .AddSqlServer(sqlConStr);

        if (redisConStr is not null)
        {
            health.AddRedis(redisConStr);
        }

        services
            .AddHealthChecksUI()
            .AddInMemoryStorage();

        return services;
    }

    public static WebApplication UseDefaultHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/_health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecksUI(o =>
        {
            o.UIPath = "/_health-ui";
            o.ApiPath = "/_health-api";
        });

        return app;
    }
}