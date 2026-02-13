using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Tests.Ticket.Builders;

namespace Tickefy.Application.Tests.Tickets.TicketActions;

public class CanExecutePositiveTests
{
    public static TheoryData<TicketAction, Domain.Ticket.Ticket, bool, bool, bool, bool> Cases => new()
    {
        { TicketAction.Cancel, TicketBuilder.New().InCreatedState(), true, false, false, false},
        { TicketAction.Cancel, TicketBuilder.New().InCreatedState(), false, true, false, false}, 
        
        { TicketAction.Take, TicketBuilder.New().InCreatedState(), false, true, false, true}, 
        
        { TicketAction.StartWork, TicketBuilder.New().InAssignedState(), false, false, true, false}, 
        
        { TicketAction.Reopen, TicketBuilder.New().InAssignedState(), false, true, false, false}, 
        
        { TicketAction.Publish, TicketBuilder.New().InAssignedState(), false, true, false, false},
        
        { TicketAction.Accept, TicketBuilder.New().InAssignedState(), false, true, false, false},
        
        { TicketAction.Complete, TicketBuilder.New().InInProgressState(), false, false, true, false},
        
        { TicketAction.Fail, TicketBuilder.New().InAssignedState(), true, false, false, false},
    };
    
    [Theory]
    [MemberData(nameof(Cases))]
    public void CanExecute_Should_ReturnTrue_When_Parameters_Satisfies(
        TicketAction action, 
        Domain.Ticket.Ticket ticket, 
        bool isAdmin, 
        bool isRequester, 
        bool isAssignedAgent, 
        bool isAgent)
    {
        action.CanExecute(ticket, isAdmin, isRequester, isAssignedAgent, isAgent).Should().BeTrue();
    }
}