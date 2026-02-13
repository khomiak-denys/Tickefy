namespace Tickefy.Application.Ticket.Common;

public record TicketActionResult
(
    string Key,
    bool RequireReason
);
