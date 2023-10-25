using ApiBestPracticesExample.Contracts;
using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using ILogger = Serilog.ILogger;

namespace ApiBestPracticesExample.Infrastructure.Database;
public static class AppDbContextSeed
{
	public const string DefaultUserPassword = "Qwerty123$";
	public static Task SeedAsync(this AppDbContext context, IWebHostEnvironment env, ILogger logger)
	{
		var policy = CreatePolicy(logger, nameof(AppDbContextSeed));

		return policy.ExecuteAsync(async () =>
		{

			if (!await context.Roles.AnyAsync())
			{
				await context.Roles.AddRangeAsync(new List<Role>()
				{
					new()
					{
						Name = Roles.Admin
					},
					new()
					{
						Name =  Roles.User
					}
				});
				await context.SaveChangesAsync();
			}
			if (!await context.Users.AnyAsync())
			{
				await context.Users.AddRangeAsync(new List<User>()
				{
					new()
					{
						Email = "admin@gmail.com",
						PasswordHash = PasswordEncrypter.HashPassword(DefaultUserPassword),
						RoleName = Roles.Admin
					},
					new()
					{
						Email = "user@gmail.com",
						PasswordHash=PasswordEncrypter.HashPassword(DefaultUserPassword),
						RoleName = Roles.User
					}
				});
				await context.SaveChangesAsync();
			}

		});
	}
	private static AsyncRetryPolicy CreatePolicy(ILogger logger, string prefix, int retries = 3)
	{
		return Policy.Handle<SqlException>().
			WaitAndRetryAsync(
				retryCount: retries,
				sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
				onRetry: (exception, timeSpan, retry, ctx) =>
				{
					logger.Warning(exception, "[{Prefix}] Error seeding database (attempt {Retry} of {Retries})", prefix, retry, retries);
				}
			);
	}
}
