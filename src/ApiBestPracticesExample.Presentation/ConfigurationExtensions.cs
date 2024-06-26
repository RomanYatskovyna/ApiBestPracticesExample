﻿namespace ApiBestPracticesExample.Presentation;

public static class ConfigurationExtensions
{
    public static string GetRequiredValue(this IConfiguration configuration, string name)
    {
        return configuration[name] ?? throw new InvalidOperationException(
            $"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");
    }

    public static TValue GetRequiredValue<TValue>(this IConfiguration configuration, string name)
    {
        return configuration.GetRequiredSection(name).Get<TValue>() ?? throw new InvalidOperationException(
            $"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");
    }

    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name) ?? throw new InvalidOperationException(
            $"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":ConnectionStrings:" + name : "ConnectionStrings:" + name)}");
    }

    public static IServiceCollection AddOptionsWithValidation<TOptions>(this IServiceCollection services,
        string configurationName) where TOptions : class
    {
        services.AddOptionsWithValidateOnStart<TOptions>()
            .BindConfiguration(configurationName)
            .ValidateDataAnnotations();

        return services;
    }
}