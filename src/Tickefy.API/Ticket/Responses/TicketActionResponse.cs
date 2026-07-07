using Tickefy.Application.Tickets.Common;

namespace Tickefy.API.Ticket.Responses;

public record TicketActionResponse
(
    string Key,
    bool RequireReason
)
{
    public static TicketActionResponse FromResult(TicketActionResult result) => new(
        result.Key,
        result.RequireReason
    );
}
