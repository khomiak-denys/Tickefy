using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.User.UpdateProfile
{
    public class UpdateProfileCommand : ICommand<Result>
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
