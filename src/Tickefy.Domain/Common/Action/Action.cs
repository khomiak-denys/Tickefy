using System.Runtime.InteropServices;

namespace Tickefy.Domain.Common.Action;

public enum TicketAction
{
    Cancel,
    Take,
    Start, 
    Accept,
    Reopen,
    Publish,
    Complete, 
    Fail
}