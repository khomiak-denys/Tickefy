using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.UserRole;

namespace Tickefy.Application.Ticket.Common.Helpers;

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

    public static bool CanExecute(this TicketAction action, bool isAdmin, bool isRequester, bool isAssignedAgent, bool isAgent)
    {
        return action switch
        {
            TicketAction.Cancel => isAdmin,
            TicketAction.Take => isAgent,
            TicketAction.StartWork => isAssignedAgent,
            TicketAction.Reopen => isRequester,
            TicketAction.Publish => isRequester,
            TicketAction.Complete => isRequester,
            TicketAction.Fail => isAdmin,
            _ => true
        };

    }
}