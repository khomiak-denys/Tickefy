namespace Tickefy.Domain.User
{
    internal interface IUserRepository
    {
        public void Add(User user);
        public Task<User?> GetByLoginAsync(string login);
    }
}
