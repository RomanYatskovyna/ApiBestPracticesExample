using ApiBestPracticesExample.Contracts;
using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ApiBestPracticesExample.Infrastructure.Database;
public static class AppDbContextSeeder
{
	public const string DefaultUserPassword = "Qwerty123$";
	public static async Task SeedDataAsync(this AppDbContext context, ILogger logger)
	{
		await SeedRolesAsync();
		await SeedEntityAsync(new User
		{
			Email = "ReadOnlyUser@gmail.com",
			PasswordHash = PasswordEncrypter.HashPassword(DefaultUserPassword),
			RoleName = Roles.Admin
		}, new User
		{
			Email = "ReadOnlyAdmin@gmail.com",
			PasswordHash = PasswordEncrypter.HashPassword(DefaultUserPassword),
			RoleName = Roles.User
		});

		async Task SeedEntityAsync<TEntity>(params TEntity[] data) where TEntity : class
		{
			var set = context.Set<TEntity>();
			if (!await set.AnyAsync())
			{
				await set.AddRangeAsync(data);
				await context.SaveChangesAsync();
				logger.Information("{EntityName} seeded successfully", typeof(TEntity).Name);
			}
		}

		async Task SeedRolesAsync()
		{
			var roleFields = typeof(Roles).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			var roleNames = roleFields.Select(r => r.GetValue(null)!.ToString());
			foreach (var roleName in roleNames)
			{
				if (!await context.Roles.AnyAsync(role => role.Name == roleName))
				{
					await context.Roles.AddAsync(new Role
					{
						Name = roleName
					});
					logger.Information("Roles {RoleName} seeded successfully", roleName);
				}
			}
			await context.SaveChangesAsync();
		}
	}
}
