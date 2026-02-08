using Tickefy.Domain.Ticket;

namespace Tickefy.Domain.Tests.Ticket.Builders;

public class TicketBuilder
{
    private readonly string _title = "some title";
    private readonly string _description = "some description";
    
    public static TicketBuilder New() => new TicketBuilder();
    
    public Domain.Ticket.Ticket InDraftState()
    {
        return Domain.Ticket.Ticket.CreateDraft(_title, _description, new UserId(), DateTime.UtcNow);
    }

    public Domain.Ticket.Ticket InCreatedState()
    {
        return Domain.Ticket.Ticket.Create(_title, _description, new UserId(), DateTime.UtcNow);
    }

    public Domain.Ticket.Ticket InAssignedState()
    {
        var ticket = InCreatedState();
        ticket.Take(new UserId(), new TeamId());
        return ticket;
    }

    public Domain.Ticket.Ticket InInProgressState()
    {
        var ticket = InAssignedState();
        ticket.StartWork();
        return ticket;
    }

    public Domain.Ticket.Ticket InCompletedState()
    {
        var ticket = InInProgressState();
        ticket.Complete();
        return ticket;
    }

    public Domain.Ticket.Ticket InReopenedState()
    {
        var ticket = InCompletedState();
        ticket.Reopen();
        return ticket;
    }

    public Domain.Ticket.Ticket InCanceledState()
    {
        var ticket = InCreatedState();
        ticket.Cancel();
        return ticket;
    }

    public Domain.Ticket.Ticket InFailedState()
    {
        var ticket = InInProgressState();
        ticket.Fail();
        return ticket;
    }

    public Domain.Ticket.Ticket InAcceptedState()
    {
        var ticket = InCompletedState();
        ticket.Accept();
        return ticket;
    }
}