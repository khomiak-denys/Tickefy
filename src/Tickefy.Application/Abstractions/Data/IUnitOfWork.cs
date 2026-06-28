namespace Tickefy.Application.Abstractions.Data
{
    /// <summary>
    /// Coordinates persistence changes across repositories.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves all pending changes to the underlying data store.
        /// </summary>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
