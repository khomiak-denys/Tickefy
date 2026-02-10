namespace Tickefy.Application.Ticket.Common;

public record ActionResult
(
    string Key,
    bool RequireReason
);
