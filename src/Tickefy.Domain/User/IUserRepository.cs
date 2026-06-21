using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.User
{
    /// <summary>
    /// Provides persistence operations for users.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Adds a user to the repository.
        /// </summary>
        /// <param name="user">The user to add.</param>
        public void Add(User user);

        /// <summary>
        /// Gets all users.
        /// </summary>
        public Task<List<User>> GetAll();

        /// <summary>
        /// Gets a user by their identifier.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        public Task<User?> GetByIdAsync(UserId id);

        /// <summary>
        /// Deletes a user from the repository.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        public void Delete(User user);

        /// <summary>
        /// Gets a user by their login.
        /// </summary>
        /// <param name="login">The user login.</param>
        public Task<User?> GetByLoginAsync(string login);
    }
}
