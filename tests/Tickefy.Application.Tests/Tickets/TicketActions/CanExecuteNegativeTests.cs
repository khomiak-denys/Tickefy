using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Tests.Ticket.Builders;

namespace Tickefy.Application.Tests.Tickets.TicketActions;

public class CanExecuteNegativeTests
{
    public static TheoryData<TicketAction, Domain.Tickets.Ticket, bool, bool, bool, bool> Cases => new()
    {
        { TicketAction.Cancel, TicketBuilder.New().InCreatedState(), false, false, false, false},
        { TicketAction.Cancel, TicketBuilder.New().InAssignedState(), false, true, false, false},
        { TicketAction.Cancel, TicketBuilder.New().InAssignedState(), false, true, true, false},

        { TicketAction.Take, TicketBuilder.New().InCreatedState(), false, false, false, false},

        { TicketAction.StartWork, TicketBuilder.New().InAssignedState(), false, false, false, false},

        { TicketAction.Reopen, TicketBuilder.New().InAssignedState(), false, false, false, false},

        { TicketAction.Publish, TicketBuilder.New().InAssignedState(), false, false, false, false},

        { TicketAction.Accept, TicketBuilder.New().InAssignedState(), false, false, false, false},

        { TicketAction.Complete, TicketBuilder.New().InInProgressState(), false, false, false, false},

        { TicketAction.Fail, TicketBuilder.New().InAssignedState(), false, false, false, false},
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanExecute_Should_ReturnFalse_When_Parameters_Dissatisfies(
        TicketAction action,
        Domain.Tickets.Ticket ticket,
        bool isAdmin,
        bool isRequester,
        bool isAssignedAgent,
        bool isAgent)
    {
        action.CanExecute(ticket, isAdmin, isRequester, isAssignedAgent, isAgent).Should().BeFalse();
    }
}
