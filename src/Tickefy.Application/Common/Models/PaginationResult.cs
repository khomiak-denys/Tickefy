namespace Tickefy.Application.Common.Models;

using System.Collections.Generic;

/// <summary>
/// Represents a paginated collection of items returned from the application layer.
/// </summary>
/// <typeparam name="T">The type of items in the pagination result.</typeparam>
public class PaginationResult<T>
{
    /// <summary>
    /// Gets the items for the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Gets the current page number (1-indexed).
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Gets the maximum number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items available across all pages.
    /// </summary>
    public int TotalCount { get; }

    public PaginationResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static PaginationResult<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        return new PaginationResult<T>(items, page, pageSize, totalCount);
    }
}
