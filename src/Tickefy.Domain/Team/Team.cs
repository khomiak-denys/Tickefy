using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Team
{
    public class Team : EntityBase<TeamId>
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public Category Category { get; private set; }
        public UserId ManagerId { get; private set; }
        public Domain.User.User Manager {  get; private set; }
        public List<Domain.User.User> Members { get; private set; } = new();

        private Team() { }

        private Team(string name, string? description)
        {
            Id = new TeamId();
            Name = name;
            Description = description;
        }

        public static Team Create(string name, string? description)
        {
            var team = new Team(name, description);
            team.OnCreate();
            return team;
        }
        public void AddMember(Domain.User.User user)
        {
            if (Members.Any(m => m.Id == user.Id))
                return;

            Members.Add(user);
            OnModify();
        }
        public void RemoveMember(Domain.User.User user)
        {
            Members.Remove(user);
            OnModify();
        }

        public void SetCategory(Category category)
        {
            Category = category;
        }

        public void SetManager(UserId id)
        {
            ManagerId = id;
        }
    }
}
