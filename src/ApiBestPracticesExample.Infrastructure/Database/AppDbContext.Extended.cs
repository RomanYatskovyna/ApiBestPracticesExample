namespace ApiBestPracticesExample.Infrastructure.Database;
public partial class AppDbContext
{
	partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyGlobalFilters("IsDeleted", false);
		modelBuilder.ApplyGlobalFilters<DateTime?>("DeleteDate", null);
	}
}
