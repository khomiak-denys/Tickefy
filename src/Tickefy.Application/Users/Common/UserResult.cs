using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.Common
{
    public record UserResult(
        Guid Id,
        string FirstName,
        string LastName
        )
    {
        public static UserResult FromEntity(User user) => new(
            user.Id.Value,
            user.FirstName,
            user.LastName
        );
    }
}
