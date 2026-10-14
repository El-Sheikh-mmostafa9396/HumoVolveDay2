using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace HumoVolveDay2.Helpers
{
    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }

    public static class PaginationHelper
    {
        /// <summary>
        /// Creates a paged result from an IQueryable source. Caller should apply ordering before calling.
        /// </summary>
        public static async Task<PagedResult<TResult>> ToPagedResultAsync<TSource, TResult>(this IQueryable<TSource> query,
            int pageNumber, int pageSize, Expression<Func<TSource, TResult>> selector)
        {
            var result = new PagedResult<TResult>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await query.CountAsync()
            };

            if (pageNumber < 1 || pageSize < 1)
            {
                result.Items = new List<TResult>();
                return result;
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(selector)
                .ToListAsync();

            result.Items = items;
            return result;
        }
    }
}
