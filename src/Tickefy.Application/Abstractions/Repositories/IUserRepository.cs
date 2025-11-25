namespace Tickefy.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        public void Add(Domain.User.User user);
        public Task<List<Domain.User.User>> GetAll();
        public Task<Domain.User.User?> GetByLoginAsync(string login, CancellationToken cancellationToken);
    }
}
