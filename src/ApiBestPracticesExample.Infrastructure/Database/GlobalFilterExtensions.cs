using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApiBestPracticesExample.Infrastructure.Database;
public static class GlobalFilterExtensions
{
	public static void ApplyGlobalFilters<T>(this ModelBuilder
		modelBuilder, string propertyName, T value)
	{
		var entityTypes = modelBuilder.Model.GetEntityTypes().ToList();
		foreach (var entityType in entityTypes)
		{
			var findProperty = entityType.FindProperty(propertyName);
			if (findProperty is not null && findProperty.ClrType == typeof(T))
			{
				var newParam = Expression.Parameter(entityType.ClrType);
				var filter = Expression.Lambda(Expression.Equal(Expression.Property(newParam, propertyName), Expression.Constant(value)), newParam);

				var entity = modelBuilder.Entity(entityType.ClrType);
				entity.HasQueryFilter(filter);

				var navigations = entityType.GetNavigations().Where(item => item.ForeignKey.IsUnique);
				foreach (var navigation in navigations)
				{
					var targetEntity = navigation.ForeignKey.DeclaringEntityType;
					newParam = Expression.Parameter(targetEntity.ClrType);

					var memberAccess = CreateMemberAccess(newParam, entityType.ClrType.Name + "." + propertyName);
					filter = Expression.Lambda(Expression.Equal(memberAccess, Expression.Constant(value)), newParam);

					modelBuilder.Entity(targetEntity.ClrType).HasQueryFilter(filter);
				}
			}
		}
	}

	private static Expression CreateMemberAccess(Expression target, string selector)
	{
		return selector.Split('.').Aggregate(target, Expression.PropertyOrField);
	}
}
