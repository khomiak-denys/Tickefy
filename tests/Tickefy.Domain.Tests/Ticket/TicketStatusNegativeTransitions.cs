using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.Action;

namespace Tickefy.Domain.Tests.Ticket;

public class TicketStatusNegativeTransitions
{
    [Theory]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenDraft(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.CreateDraft("some title", "some description", new UserId(), new DateTime());

        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }
    
    [Theory]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenCreated(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());

        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenAssigned(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());

        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    public void Action_Should_ThrowException_WhenInProgress(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();

        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenCompleted(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Complete();

        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenReopened(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Complete();
        ticket.Reopen();

        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenCanceled(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Cancel();
        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }
    
    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenFailed(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Fail();
        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }
    
    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenAccepted(TicketAction action)
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Complete();
        ticket.Accept();
        Invoke(ticket, action).Should().Throw<ForbiddenException>().WithMessage("Invalid action");
    }

    private Action Invoke(Domain.Ticket.Ticket ticket, TicketAction action)
    {
        return action switch
        {
            TicketAction.Cancel => () => ticket.Cancel(),
            TicketAction.Take => () => ticket.Take(new UserId(), new TeamId()),
            TicketAction.StartWork => () => ticket.StartWork(),
            TicketAction.Accept => () => ticket.Accept(),
            TicketAction.Reopen => () => ticket.Reopen(),
            TicketAction.Publish => () => ticket.Publish(),
            TicketAction.Complete => () => ticket.Complete(),
            TicketAction.Fail => () => ticket.Fail(),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
    }
    
}