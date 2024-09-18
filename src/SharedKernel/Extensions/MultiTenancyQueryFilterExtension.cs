using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using SharedKernel.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SharedKernel.Extensions;

public static class MultiTenancyQueryFilterExtension
{
    public static void AddMultiTenancyQueryFilter(
        this IMutableEntityType entityData, Expression<Func<IMultiTenant, bool>> filterExpr)
    {
        var parameter = Expression.Parameter(entityData.ClrType);
        var body = ReplacingExpressionVisitor.Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
        var lambdaExpression = Expression.Lambda(body, parameter);
        entityData.SetQueryFilter(lambdaExpression);
    }
}