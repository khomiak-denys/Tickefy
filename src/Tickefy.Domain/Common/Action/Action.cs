using System.Runtime.InteropServices;

namespace Tickefy.Domain.Common.Action;

public enum Action
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