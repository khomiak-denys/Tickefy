using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Teams.Create
{
    public class CreateTeamCommand : ICommand<Result>
    {
        public UserId UserId { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
        public Category Category { get; init; }

        public CreateTeamCommand(UserId userId, string name, string? description, Category category)
        {
            UserId = userId;
            Name = name;
            Description = description;
            Category = category;
        }
    }
}
