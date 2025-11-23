using Tickefy.Domain.User;
namespace Tickefy.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        public void Add(User user);
        public Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken);
    }
}
