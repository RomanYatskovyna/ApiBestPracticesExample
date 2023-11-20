using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBestPracticesExample.Infrastructure.Database;
public partial class AppDbContext
{
	partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyGlobalFilters("IsDeleted", false);
		modelBuilder.ApplyGlobalFilters<DateTime?>("DeleteDate", null);
	}
}
