namespace Tickefy.API.Ticket.Responses;

public record TicketActionResponse
(
    string Key,
    bool RequireReason
);