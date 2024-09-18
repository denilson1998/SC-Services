using SharedKernel.DataFilters.Parameters;
using System.Linq.Expressions;

namespace SharedKernel.DataFilters.Sorting;

public static class SortingHelper
{
    public static IQueryable<T> ApplySortOptions<T>(List<SortParameters> sortOptions, IQueryable<T> query)
    {
        var isFirst = true;
        string command;
        var type = typeof(T);
        foreach (SortParameters option in sortOptions)
        {
            if (isFirst)
            {
                command = Convert.ToBoolean(option.Direction) ? "OrderByDescending" : "OrderBy";
                isFirst = false;
            }
            else
            {
                command = Convert.ToBoolean(option.Direction) ? "ThenByDescending" : "ThenBy";
            }
            var property = type.GetProperty(option.Field);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                query.Expression, Expression.Quote(orderByExpression));
            query = (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExpression);
        }
        return query;
    }
}
