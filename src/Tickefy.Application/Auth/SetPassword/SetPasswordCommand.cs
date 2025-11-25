using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Auth.SetPassword
{
    public class SetPasswordCommand : ICommand
    {
        public UserId UserId { get; init; }
        public string OldPassword { get; init; }
        public string NewPassword { get; init; }
        public SetPasswordCommand(UserId userId, string oldPassword, string newPassword)
        {
            UserId = userId;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
}
