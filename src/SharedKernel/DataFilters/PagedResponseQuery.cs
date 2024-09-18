using SharedKernel.AbstractEntities;
using SharedKernel.Contracts;

namespace SharedKernel.DataFilters;

public static class PagedResponseQuery
{
    public static IQueryable<AuditableEntity> QueryPaged(this IQueryable<AuditableEntity> source, ListQueryRequest request, bool overrideMaxLimit = false)
    {
        if (request.Since is not null)
        {
            source = source.Where(q => q.CreatedAt >= request.Since);
        }

        if (request.Before is not null)
        {
            source = source.Where(q => q.CreatedAt <= request.Before);
        }

        if (request.Skip < 0)
        {
            request.Skip = 0;
        }

        if (request.Limit < 0 || (request.Limit > 100 && !overrideMaxLimit))
        {
            request.Limit = 100;
        }

        return source.Skip(request.Skip).Take(request.Limit).OrderByDescending(q => q.Id);
    }
}