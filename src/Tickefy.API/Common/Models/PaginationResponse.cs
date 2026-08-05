namespace Tickefy.API.Common.Models;

using System.Collections.Generic;
using Tickefy.Application.Common.Models;

    /// <summary>
    /// Represents a paginated HTTP response containing a subset of items and pagination metadata.
    /// </summary>
    /// <typeparam name="T">The type of the items in the response.</typeparam>
    public class PaginationResponse<T>
    {
        /// <summary>
        /// Gets or initializes the items for the current page.
        /// </summary>
        public IReadOnlyList<T> Items { get; init; } = new List<T>();

        /// <summary>
        /// Gets or initializes the current page number (1-indexed).
        /// </summary>
        public int PageNumber { get; init; }

        /// <summary>
        /// Gets or initializes the maximum number of items per page.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Gets or initializes the total number of items available across all pages.
        /// </summary>
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
