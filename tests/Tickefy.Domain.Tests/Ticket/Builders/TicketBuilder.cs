namespace Tickefy.Domain.Tests.Ticket.Builders;

public class TicketBuilder
{
    private readonly string _title = "some title";
    private readonly string _description = "some description";

    public static TicketBuilder New() => new TicketBuilder();

    public Domain.Tickets.Ticket InDraftState()
    {
        return Domain.Tickets.Ticket.CreateDraft(_title, _description, new UserId(), DateTime.UtcNow);
    }

    public Domain.Tickets.Ticket InCreatedState()
    {
        return Domain.Tickets.Ticket.Create(_title, _description, new UserId(), DateTime.UtcNow);
    }

    public Domain.Tickets.Ticket InAssignedState()
    {
        var ticket = InCreatedState();
        ticket.Take(new UserId(), new TeamId());
        return ticket;
    }

    public Domain.Tickets.Ticket InInProgressState()
    {
        var ticket = InAssignedState();
        ticket.StartWork();
        return ticket;
    }

    public Domain.Tickets.Ticket InCompletedState()
    {
        var ticket = InInProgressState();
        ticket.Complete();
        return ticket;
    }

    public Domain.Tickets.Ticket InReopenedState()
    {
        var ticket = InCompletedState();
        ticket.Reopen();
        return ticket;
    }

    public Domain.Tickets.Ticket InCanceledState()
    {
        var ticket = InCreatedState();
        ticket.Cancel();
        return ticket;
    }

    public Domain.Tickets.Ticket InFailedState()
    {
        var ticket = InInProgressState();
        ticket.Fail();
        return ticket;
    }

    public Domain.Tickets.Ticket InAcceptedState()
    {
        var ticket = InCompletedState();
        ticket.Accept();
        return ticket;
    }
}
