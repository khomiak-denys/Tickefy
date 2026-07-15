using Tickefy.Application.Auth.Register;

namespace Tickefy.API.Auth.Requests
{
    public class RegisterUserRequest
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Login { get; init; }
        public required string Password { get; init; }

        public RegisterUserCommand ToCommand()
        {
            return new RegisterUserCommand
            {
                Login = Login,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName
            };
        }

    }
}
