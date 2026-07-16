using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.Common.Helpers;

public static class ActionHelper
{
    public static bool RequireReason(this TicketAction action)
    {
        return action switch
        {
            TicketAction.Cancel => true,
            TicketAction.Fail => true,
            TicketAction.Reopen => true,
            _ => false
        };
    }

    public static bool CanExecute(this TicketAction action, Domain.Tickets.Ticket ticket, bool isAdmin, bool isRequester, bool isAssignedAgent, bool isAgent)
    {
        return action switch
        {
            TicketAction.Cancel => isAdmin || (ticket.Status == Status.Created && isRequester),
            TicketAction.Take => isAgent,
            TicketAction.StartWork => isAssignedAgent,
            TicketAction.Reopen => isRequester,
            TicketAction.Publish => isRequester,
            TicketAction.Accept => isRequester,
            TicketAction.Complete => isAssignedAgent,
            TicketAction.Fail => isAdmin,
            _ => false
        };
    }

    public static bool CanExecute(this TicketAction action, Domain.Tickets.Ticket ticket, UserId userId, IEnumerable<string> roles)
    {
        var roleSet = roles as ISet<string> ?? roles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var isAgent = roleSet.Contains(nameof(UserRoles.Agent));
        var isRequester = ticket.RequesterId == userId;
        var isAdmin = roleSet.Contains(nameof(UserRoles.Admin));
        var isAssignedAgent = ticket.AssignedAgentId == userId && isAgent;

        return action.CanExecute(ticket, isAdmin, isRequester, isAssignedAgent, isAgent);
    }
}
