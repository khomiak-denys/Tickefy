using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Teams;

namespace Tickefy.Domain.Users
{
    public class User : EntityBase<UserId>
    {
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string Login { get; init; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRoles Role { get; private set; }

        public TeamId? TeamId { get; private set; } = null;
        public Team? Team { get; private set; } = null;

        private User() { }

        public static User Create(string firstName, string lastName, string login, string passwordHash)
        {
            var user = new User(firstName, lastName, login, passwordHash, UserRoles.Requester);
            user.OnCreate();
            return user;
        }

        private User(string firstName, string lastName, string login, string passwordHash, UserRoles role)
        {
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

        public void SetTeam(Team? team)
        {
            TeamId = team?.Id;
            Team = team;
            OnModify();
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
