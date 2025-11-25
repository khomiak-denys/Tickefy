using Tickefy.Application.Auth.SetPassword;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Auth.Requests
{
    public class SetPasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public SetPasswordCommand ToCommand(UserId userId)
        {
            return new SetPasswordCommand(userId, OldPassword, NewPassword);
        }
    }
}
