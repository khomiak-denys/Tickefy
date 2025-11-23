namespace Tickefy.API.Auth.Responses
{
    public record LoginResponse
    (
        Guid Id,
        string FirstName,
        string LastName,
        string Login,
        string Token 
    );
}
