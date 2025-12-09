using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.User
{
    public interface IUserRepository
    {
        public void Add(User user);
        public Task<List<User>> GetAll();
        public Task<User?> GetByIdAsync(UserId id);
        public void Delete(User user);
        public Task<User?> GetByLoginAsync(string login);
    }
}
