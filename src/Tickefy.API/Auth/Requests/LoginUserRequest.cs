using Tickefy.Application.Auth.Login;

namespace Tickefy.API.Auth.Requests
{
    public class LoginUserRequest
    {
        public required string Login { get; init; }
        public required string Password { get; init; }

        public LoginUserCommand ToCommand()
        {
            return new LoginUserCommand
            {
                Login = Login,
                Password = Password
            };
        }
    }
}
