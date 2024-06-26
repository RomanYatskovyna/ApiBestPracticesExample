﻿using System.Linq.Expressions;

namespace ApiBestPracticesExample.Infrastructure.Database;

public static class GlobalFilterExtensions
{
    public static void ApplyGlobalFilters<T>(this ModelBuilder
        modelBuilder, string propertyName, T value)
    {
        var entityTypes = modelBuilder.Model
            .GetEntityTypes()
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var property = entityType.FindProperty(propertyName);

            if (property is null || property.ClrType != typeof(T))
            {
                continue;
            }

            var newParam = Expression.Parameter(entityType.ClrType);
            var filter = Expression.Lambda(
                Expression.Equal(Expression.Property(newParam, propertyName), Expression.Constant(value)),
                newParam);

            var entity = modelBuilder.Entity(entityType.ClrType);
            entity.HasQueryFilter(filter);

            var entityNavigations = entityType
                .GetNavigations()
                .Where(item => item.ForeignKey.IsUnique);

            foreach (var navigation in entityNavigations)
            {
                var targetEntity = navigation.ForeignKey.DeclaringEntityType;
                newParam = Expression.Parameter(targetEntity.ClrType);

                var memberAccess = CreateMemberAccess(newParam, entityType.ClrType.Name + "." + propertyName);

                filter = Expression.Lambda(Expression.Equal(memberAccess, Expression.Constant(value)), newParam);

                modelBuilder.Entity(targetEntity.ClrType).HasQueryFilter(filter);
            }
        }
    }

    private static Expression CreateMemberAccess(Expression target, string selector)
    {
        return selector
            .Split('.')
            .Aggregate(target, Expression.PropertyOrField);
    }
}