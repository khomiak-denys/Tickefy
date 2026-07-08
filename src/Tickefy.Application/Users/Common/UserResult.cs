using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.Common
{
    public record UserResult(
        Guid Id,
        string FirstName,
        string LastName
        )
    {
        public static UserResult FromEntity(User? user) => user is not null ? new(
            user.Id.Value,
            user.FirstName,
            user.LastName
        ) : new(Guid.Empty, string.Empty, string.Empty);
    }
}
