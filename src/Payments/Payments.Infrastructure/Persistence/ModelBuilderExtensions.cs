using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using SharedKernel.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using Payments.Domain;
using static System.Linq.Expressions.Expression;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    public static void AddQueryFilters(this ModelBuilder modelBuilder,
        Expression<Func<ISoftDelete, bool>> deletedFilter,
        Expression<Func<IMultiTenant, bool>> tenantFilter)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(et =>
                typeof(ISoftDelete).IsAssignableFrom(et.ClrType) || typeof(IMultiTenant).IsAssignableFrom(et.ClrType))
            .Where(et => et.ClrType != typeof(QrPayment))
            .Where(et => et.ClrType != typeof(VoucherPayment))
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var parameter = Parameter(entityType.ClrType);
            var softDeleteExpression = PrepareExpression(deletedFilter, entityType, parameter);
            var multiTenantExpression = PrepareExpression(tenantFilter, entityType, parameter);

            Expression queryFilterExpression = null;
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType) &&
                typeof(IMultiTenant).IsAssignableFrom(entityType.ClrType))
            {
                queryFilterExpression = AndAlso(softDeleteExpression, multiTenantExpression);
            }
            else
            {
                queryFilterExpression = typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType)
                    ? softDeleteExpression
                    : multiTenantExpression;
            }

            entityType.SetQueryFilter(Lambda(queryFilterExpression, parameter));
        }
    }

    private static Expression PrepareExpression<T>(Expression<Func<T, bool>> originalExpression,
        IMutableEntityType entityType, ParameterExpression parameter)
    {
        if (typeof(T).IsAssignableFrom(entityType.ClrType))
        {
            return ReplacingExpressionVisitor.Replace(originalExpression.Parameters.First(), parameter,
                originalExpression.Body);
        }

        return originalExpression;
    }
}