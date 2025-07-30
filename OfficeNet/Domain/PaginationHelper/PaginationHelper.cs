using Microsoft.EntityFrameworkCore;

namespace OfficeNet.Domain.PaginationModel
{
    public static class PaginationHelper
    {
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
        this IQueryable<T> source, PaginationRequest request)
        {
            var count = await source.CountAsync();

            var items = await source
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

}
