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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
