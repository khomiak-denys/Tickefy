using Tickefy.Application.Auth.SetPassword;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Auth.Requests
{
    public class SetPasswordRequest
    {
        public required string OldPassword { get; init; }
        public required string NewPassword { get; init; }

        public SetPasswordCommand ToCommand(UserId userId)
        {
            return new SetPasswordCommand(userId, OldPassword, NewPassword);
        }
    }
}
