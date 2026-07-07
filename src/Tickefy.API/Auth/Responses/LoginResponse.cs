using Tickefy.Application.Auth.Common;

namespace Tickefy.API.Auth.Responses
{
    public record LoginResponse
    (
        Guid Id,
        string FirstName,
        string LastName,
        string Login,
        string Token
    )
    {
        public static LoginResponse FromResult(LoginResult result) => new(
            result.Id,
            result.FirstName,
            result.LastName,
            result.Login,
            result.Token
        );
    }
}
