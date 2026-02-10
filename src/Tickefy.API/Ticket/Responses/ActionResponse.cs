namespace Tickefy.API.Ticket.Responses;

public record ActionResponse
(
    string Key,
    bool RequireReason
);