namespace Tickefy.API.Team.Responses
{
    public record TeamResponse
    (
        Guid Id,
        string Name,
        string Category,
        Guid ManagerId
    );
}
