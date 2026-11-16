using System.Linq.Expressions;
using Core.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds <c>WHERE IsDeleted = 0</c> as a global query filter to every soft-deletable root entity.
    /// Use <c>IgnoreQueryFilters()</c> in a specification to include deleted rows.
    /// </summary>
    public static ModelBuilder ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        var softDeletable = modelBuilder.Model.GetEntityTypes()
            .Where(type => typeof(ISoftDeletableEntity).IsAssignableFrom(type.ClrType)
                && type.BaseType is null
                && !type.IsOwned());

        foreach (var entityType in softDeletable)
        {
            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var isDeleted = Expression.Property(parameter, nameof(ISoftDeletableEntity.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(isDeleted), parameter);
            entityType.SetQueryFilter(filter);
        }

        return modelBuilder;
    }
}
