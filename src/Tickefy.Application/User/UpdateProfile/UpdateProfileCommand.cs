using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.User.UpdateProfile
{
    public class UpdateProfileCommand : ICommand
    {
        public UserId UserId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public UpdateProfileCommand(UserId userId, string firstName, string lastName)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
