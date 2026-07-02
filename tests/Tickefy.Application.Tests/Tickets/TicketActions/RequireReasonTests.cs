using Tickefy.Application.Tickets.Common.Helpers;
using Tickefy.Domain.Common.Action;

namespace Tickefy.Application.Tests.Tickets.TicketActions;

public class RequireReasonTests
{
    [Theory]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.Fail)]
    [InlineData(TicketAction.Reopen)]
    public void RequireReason_Should_BeTrue_WhenTicketAction(TicketAction action)
    {
        action.RequireReason().Should().BeTrue();
    }

    [Theory]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    public void RequireReason_Should_BeFalse_WhenTicketAction(TicketAction action)
    {
        action.RequireReason().Should().BeFalse();
    }
}
