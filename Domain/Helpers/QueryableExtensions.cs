using System.Reflection;
using Domain.Dto;

namespace Domain.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public static class QueryableExtensions
{
    
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Expression<Func<T, bool>>? filter)
    {
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return query;
    }

    public static async Task<PagedResult<T>> ToPagedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        int totalCount = await query.CountAsync();
        List<T> items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
    
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, bool descending)
    {
        if (string.IsNullOrEmpty(sortBy)) return query;

        PropertyInfo? property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property == null) return query; // Ignore invalid sort fields

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression propertyAccess = Expression.MakeMemberAccess(parameter, property);
        LambdaExpression orderByExpression = Expression.Lambda(propertyAccess, parameter);

        string methodName = descending ? "OrderByDescending" : "OrderBy";
        MethodCallExpression resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.PropertyType],
            query.Expression,
            Expression.Quote(orderByExpression)
        );

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}
