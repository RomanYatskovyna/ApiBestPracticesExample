
using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;
namespace ApiBestPracticesExample.Infrastructure.Database;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual required DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual required DbSet<Role> Roles { get; set; }

    public virtual required DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.RoleConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());

        OnModelCreatingGeneratedFunctions(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
