using ApiBestPracticesExample.Contracts;
using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ApiBestPracticesExample.Infrastructure.Database;
public static class AppDbContextSeeder
{
	public const string DefaultUserPassword = "Qwerty123$";
	public const string DefaultAdminPassword = "Qwerty123$";


	public static readonly User DefaultAdmin = new()
	{
		Email = "ReadOnlyAdmin@gmail.com",
		PasswordHash = PasswordEncrypter.HashPassword(DefaultAdminPassword),
		RoleName = SupportedRoles.Admin
	};
	public static readonly User DefaultUser = new()
	{
		Email = "ReadOnlyUser@gmail.com",
		PasswordHash = PasswordEncrypter.HashPassword(DefaultUserPassword),
		RoleName = SupportedRoles.User
	};
	public static async Task SeedDataAsync(this AppDbContext context, ILogger logger)
	{

		await SeedRolesAsync();
		DefaultAdmin.RoleNameNavigation = null;
		DefaultUser.RoleNameNavigation = null;

		await SeedEntityAsync(DefaultAdmin, DefaultUser);

		async Task SeedEntityAsync<TEntity>(params TEntity[] data) where TEntity : class
		{
			var set = context.Set<TEntity>();
		 	var lcoal= set.Local;
			if (!await set.AnyAsync())
			{
				await set.AddRangeAsync(data);
				await context.SaveChangesAsync();
				context.ChangeTracker.Clear();
				logger.Information("{EntityName} seeded successfully", typeof(TEntity).Name);
			}
		}

		async Task SeedRolesAsync()
		{
			var roleFields = typeof(SupportedRoles).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			var roleNames = roleFields.Select(r => r.GetValue(null)!.ToString()!).ToList();
			foreach (var roleName in roleNames)
			{
				if (!await context.Roles.AnyAsync(role => role.Name == roleName))
				{
					await context.Roles.AddAsync(new Role
					{
						Name = roleName
					});
					logger.Information("SupportedRoles {RoleName} seeded successfully", roleName);
					await context.SaveChangesAsync();
				}
			}
			var roleDeletedCount = await context.Roles.Where(r => !roleNames.Contains(r.Name)).ExecuteDeleteAsync();
			logger.Information("{RoleDeletedCount} SupportedRoles  deleted successfully", roleDeletedCount);

			await context.SaveChangesAsync();
		}
	}
}
