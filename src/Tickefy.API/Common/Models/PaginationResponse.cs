namespace Tickefy.API.Common.Models;

using System.Collections.Generic;
using Tickefy.Application.Common.Models;

public class PaginationResponse<T>
{
    public IReadOnlyList<T> Items { get; init; } = new List<T>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }

    public PaginationResponse() { }

    public PaginationResponse(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static PaginationResponse<T> FromResult(PaginationResult<T> result)
    {
        return new PaginationResponse<T>(result.Items, result.PageNumber, result.PageSize, result.TotalCount);
    }
}
