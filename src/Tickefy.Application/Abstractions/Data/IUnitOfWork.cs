namespace Tickefy.Application.Abstractions.Data
{
    /// <summary>
    /// Defines a contract for coordinating atomic transactions and synchronizing tracking changes across multiple domain repositories.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Asynchronously commits all pending entity state modifications tracked by the persistence context to the underlying relational database.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous save operation. The task result contains the number of individual database state entries written or modified.
        /// </returns>
        /// <remarks>
        /// This operation encapsulates all staged additions, updates, and deletions into a single atomic database transaction. If any entity constraint or concurrency violation occurs during commit, the transaction rolls back automatically and throws a persistence exception.
        /// </remarks>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
