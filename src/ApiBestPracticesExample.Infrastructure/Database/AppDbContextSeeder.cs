using ApiBestPracticesExample.Contracts;
using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
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
	public static async Task SeedDefaultDataAsync(this AppDbContext context)
	{
		await SeedRolesAsync(context);
	}

	public static async Task SeedDevelopmentTestDataAsync(this AppDbContext context)
	{

		DefaultAdmin.RoleNameNavigation = null;
		DefaultUser.RoleNameNavigation = null;
		await SeedEntityAsync(context,DefaultAdmin, DefaultUser);
	}
	private static async Task SeedEntityAsync<TEntity>(DbContext context, params TEntity[] data) where TEntity : class
	{
		var set = context.Set<TEntity>();
		if (!await set.AnyAsync())
		{
			await set.AddRangeAsync(data);
			await context.SaveChangesAsync();
			context.ChangeTracker.Clear();
		}
	}
	private static async Task SeedRolesAsync(AppDbContext context)
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
				await context.SaveChangesAsync();
			}
		}
		//var roleDeletedCount = await context.Roles.Where(r => !roleNames.Contains(r.Name)).ExecuteDeleteAsync();
		context.Roles.RemoveRange(context.Roles.Where(r => !roleNames.Contains(r.Name)));
		await context.SaveChangesAsync();
	}
}
