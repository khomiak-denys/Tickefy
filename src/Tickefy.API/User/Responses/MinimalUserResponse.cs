namespace Tickefy.API.User.Responses
{
    public record MinimalUserResponse(
        Guid id,
        string FirstName,
        string LastName
        );
}
