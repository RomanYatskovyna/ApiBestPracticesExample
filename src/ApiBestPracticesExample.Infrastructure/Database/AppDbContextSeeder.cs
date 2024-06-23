using ApiBestPracticesExample.Contracts;
using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Services;
using System.Reflection;

namespace ApiBestPracticesExample.Infrastructure.Database;

public static class AppDbContextSeeder
{
    public const string DefaultUserPassword = "Qwerty123$";
    public const string DefaultAdminPassword = "Qwerty123$";


    public static User DefaultAdmin { get; private set; } = null!;

    public static User DefaultUser { get; private set; } = null!;

    public static async Task SeedDefaultDataAsync(this AppDbContext context)
    {
        await SeedRolesAsync(context);
    }

    public static async Task SeedDevelopmentTestDataAsync(this AppDbContext context)
    {
        DefaultAdmin = new User
        {
            Email = "ReadOnlyAdmin@gmail.com",
            PasswordHash = PasswordEncrypter.HashPassword(DefaultAdminPassword),
            RoleName = SupportedRoles.Admin,
        };
        DefaultUser = new User
        {
            PhoneNumber = "+1 408-555-1234",
            Email = "ReadOnlyUser@gmail.com",
            PasswordHash = PasswordEncrypter.HashPassword(DefaultUserPassword),
            RoleName = SupportedRoles.User,
        };
        await SeedEntityAsync(context, DefaultAdmin, DefaultUser);
    }

    private static async Task SeedEntityAsync<TEntity>(DbContext context, params TEntity[] data) where TEntity : class
    {
        var set = context.Set<TEntity>();
        if (!await set.AnyAsync())
        {
            await set.AddRangeAsync(data);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedRolesAsync(AppDbContext context)
    {
        var roleFields =
            typeof(SupportedRoles).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        var roleNames = roleFields.Select(r => r.GetValue(null)!.ToString()!).ToList();

        foreach (var roleName in roleNames)
        {
            if (!await context.Roles.AnyAsync(role => role.Name == roleName))
            {
                await context.Roles.AddAsync(new Role { Name = roleName });
            }
        }

        await context.SaveChangesAsync();
    }
}