namespace Vacancies.DTOs
{
    /// <summary>
    /// Represents a paginated result set with metadata
    /// </summary>
    /// <typeparam name="T">The type of items in the result set</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The items in the current page
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indicates if there is a next page available
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Indicates if there is a previous page available
        /// </summary>
        public bool HasPreviousPage => Page > 1;
    }
}