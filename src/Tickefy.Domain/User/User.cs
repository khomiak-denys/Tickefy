using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.User
{
    public class User : EntityBase<UserId>
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Login { get; init; }
        public string PasswordHash { get; private set; }
        public UserRoles Role { get; private set; }

        public TeamId? TeamId { get; private set; } = null;
        public Domain.Team.Team? Team { get; private set; } = null;


        private User() : base (){ }
        public static User Create(string firstName, string lastName, string login, string passwordHash)
        {
            var user = new User(firstName, lastName, login, passwordHash, UserRoles.Requester);
            user.OnCreate();
            return user;
        }
        private User(string firstName, string lastName, string login, string passwordHash, UserRoles role) : base()
        {
            Id = new UserId();
            FirstName = firstName;
            LastName = lastName;
            Login = login;
            PasswordHash = passwordHash;
            Role = role;
        }

        public void SetRole(UserRoles role)
        {
            Role = role;
        }
        public void Update(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public void UpdatePassword(string password)
        {
            PasswordHash = password;
        }
    }
}
