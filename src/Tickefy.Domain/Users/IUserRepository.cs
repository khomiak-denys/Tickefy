using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Users
{
    /// <summary>
    /// Defines contract requirements for managing the lifecycle and persistence of user aggregate roots.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Stages a new user entity to be tracked by the underlying persistence context for future insertion into the database.
        /// </summary>
        /// <param name="user">
        /// The fully instantiated user aggregate root to track. Must contain a unique login and valid identifier; passing an already tracked or null entity may result in invalid state exceptions during transaction commit.
        /// </param>
        /// <remarks>
        /// This operation is synchronous and strictly in-memory; actual persistence to the underlying data store occurs only when the unit of work saves changes.
        /// </remarks>
        public void Add(User user);

        /// <summary>
        /// Asynchronously retrieves all registered users from the persistent store, eagerly loading their associated team relationships.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous query operation. The task result contains a list of all existing <see cref="User"/> entities, or an empty list if no users exist.
        /// </returns>
        /// <remarks>
        /// Be cautious when invoking this method on large datasets without pagination, as loading all users and their related team data simultaneously may consume significant memory and database bandwidth.
        /// </remarks>
        public Task<(int TotalCount, List<User> Items)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously locates and retrieves a specific user entity based on its unique domain identifier.
        /// </summary>
        /// <param name="id">
        /// The strongly-typed domain identifier of the target user. Passing an uninitialized or empty identifier will result in a null return value or query failure.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous read operation. The task result contains the matching <see cref="User"/> instance if found; otherwise, <see langword="null"/>.
        /// </returns>
        public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks an existing tracked user entity for deletion in the persistence context.
        /// </summary>
        /// <param name="user">
        /// The user entity instance to remove. The entity must currently be tracked by the data context; attempting to delete an untracked or detached instance may throw an exception or result in no-op behavior during save.
        /// </param>
        /// <remarks>
        /// Removal is deferred until the unit of work transaction is committed. Ensure that any foreign key constraints or cascading deletion behaviors are accounted for before committing.
        /// </remarks>
        public void Delete(User user);

        /// <summary>
        /// Asynchronously searches for a user entity by its unique login username.
        /// </summary>
        /// <param name="login">
        /// The unique login string identifying the user account. The search is typically case-sensitive or case-insensitive depending on database collation rules; leading or trailing whitespace should be trimmed prior to invocation.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous query operation. The task result contains the matching <see cref="User"/> entity if a record with the specified login exists; otherwise, <see langword="null"/>.
        /// </returns>
        public Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    }
}
