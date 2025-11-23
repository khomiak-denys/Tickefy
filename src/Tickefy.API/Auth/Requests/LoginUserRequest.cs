using Tickefy.Application.Auth.Login;

namespace Tickefy.API.Auth.Requests
{
    public class LoginUserRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }

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
