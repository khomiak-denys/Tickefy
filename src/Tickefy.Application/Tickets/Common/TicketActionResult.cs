namespace Tickefy.Application.Tickets.Common;

public record TicketActionResult
(
    string Key,
    bool RequireReason
);
