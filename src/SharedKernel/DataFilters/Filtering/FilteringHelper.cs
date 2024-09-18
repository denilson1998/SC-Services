using SharedKernel.Constants;
using SharedKernel.DataFilters.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.DataFilters.Filtering
{
    public static class FilteringHelper
    {
        public static IQueryable<T> ApplyFilterOptions<T>(List<FilterParameters> filterOptions, IQueryable<T> query)
        {
            if (filterOptions == null) return query;

            Expression<Func<T, bool>> finalExpression = BuildExpression<T>(filterOptions);

            if (finalExpression != null)
            {
                query = query.Where(finalExpression);
            }

            return query;
        }

        private static Expression<Func<T, bool>> BuildExpression<T>(List<FilterParameters> filterOptions,
            string concatenator = null, Expression<Func<T, bool>> finalExpression = null)
        {
            if (filterOptions == null) return finalExpression;

            var type = typeof(T);
            Expression<Func<T, bool>> semiFinalExpression = null;

            foreach (var option in filterOptions)
            {
                if (option.Children == null)
                {
                    // Get the parameter and convert to a member access of the Entity
                    var property = type.GetProperty(option.Field)!;
                    var parameter = Expression.Parameter(type, "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    // Create the constant for comparission with the correct's data type
                    var convertedValue = Convert.ChangeType(option.Value, property.PropertyType);
                    var searchArgument = Expression.Constant(convertedValue);

                    Expression<Func<T, bool>> expression = GetExpression<T>(option, parameter, propertyAccess, searchArgument);

                    if (semiFinalExpression == null)
                    {
                        semiFinalExpression = expression;
                    }
                    else if (option.Concatenator == Concatenator.And)
                    {
                        semiFinalExpression = semiFinalExpression.And<T>(expression);
                    }
                    else if (option.Concatenator == Concatenator.Or)
                    {
                        semiFinalExpression = semiFinalExpression.Or<T>(expression);
                    }
                }

                semiFinalExpression = BuildExpression(option.Children, option.Concatenator, semiFinalExpression);
            }

            if (finalExpression == null)
            {
                finalExpression = semiFinalExpression;
            }
            else if (concatenator == Concatenator.And)
            {
                finalExpression = finalExpression.And<T>(semiFinalExpression);
            }
            else if (concatenator == Concatenator.Or)
            {
                finalExpression = finalExpression.Or<T>(semiFinalExpression);
            }

            return finalExpression;
        }

        public static Expression<Func<T, bool>> GetExpression<T>(FilterParameters option,
            ParameterExpression parameter, MemberExpression propertyAccess, ConstantExpression searchArgument)
        {
            if (option.Operation is null) return null;

            Expression<Func<T, bool>> expression = null;

            // Check and build the operation
            switch (option.Operation.ToString())
            {
                case Operation.Equal:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.Equal(propertyAccess, searchArgument), parameter);
                    break;

                case Operation.NotEqual:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(propertyAccess, searchArgument), parameter);
                    break;

                case Operation.Contains:
                    var method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                    var containsExpression = Expression.Call(propertyAccess, method, searchArgument);
                    expression = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
                    break;

                case Operation.LessThan:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.LessThan(propertyAccess, searchArgument), parameter);
                    break;

                case Operation.GreaterThan:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(propertyAccess, searchArgument), parameter);
                    break;

                case Operation.LessThanOrEqual:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(propertyAccess, searchArgument), parameter);
                    break;

                case Operation.GreaterThanOrEqual:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(propertyAccess, searchArgument), parameter);
                    break;
            }

            return expression;
        }
    }

    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }

}
