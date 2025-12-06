namespace Tickefy.API.User.Responses
{
    public record MinimalUserResponse(
        Guid Id,
        string FirstName,
        string LastName
        );
}
