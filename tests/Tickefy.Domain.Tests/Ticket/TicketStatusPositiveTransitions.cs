using Tickefy.Domain.Tests.Ticket.Builders;

namespace Tickefy.Domain.Tests.Ticket;

public class TicketStatusPositiveTransitionsTests
{
    [Fact]
    public void Status_Should_BeCreated_AfterDraft()
    {
        var ticket = TicketBuilder.New().InDraftState();
        ticket.Publish(string.Empty, string.Empty,  new DateTime());
        ticket.Status.Should().Be(Status.Created);
    }

    [Fact]
    public void Status_Should_BeAssigned_AfterCreated()
    {
        var ticket = TicketBuilder.New().InCreatedState();
        ticket.Take(new UserId(), new TeamId());
        ticket.Status.Should().Be(Status.Assigned);
    }

    [Fact]
    public void Status_Should_BeInProgress_AfterAssigned()
    {
        var ticket = TicketBuilder.New().InAssignedState();
        ticket.StartWork();
        ticket.Status.Should().Be(Status.InProgress);
    }

    [Fact]
    public void Status_Should_BeInProgress_AfterReopened()
    {
        var ticket = TicketBuilder.New().InReopenedState();
        ticket.StartWork();
        ticket.Status.Should().Be(Status.InProgress);
    }

    [Fact]
    public void Status_Should_BeCompleted_AfterInProgress()
    {
        var ticket = TicketBuilder.New().InInProgressState();
        ticket.Complete();
        ticket.Status.Should().Be(Status.Completed);
    }

    [Fact]
    public void Status_Should_BeReopened_AfterCompleted()
    {
        var ticket = TicketBuilder.New().InCompletedState();
        ticket.Reopen();
        ticket.Status.Should().Be(Status.Reopened);
    }

    [Fact]
    public void Status_Should_BeAccepted_AfterCompleted()
    {
        var ticket = TicketBuilder.New().InCompletedState();
        ticket.Accept();
        ticket.Status.Should().Be(Status.Accepted);
    }

    [Fact]
    public void Status_Should_BeFailed_AfterInProgress()
    {
        var ticket = TicketBuilder.New().InInProgressState();
        ticket.Fail();
        ticket.Status.Should().Be(Status.Failed);
    }

    [Fact]
    public void Status_Should_BeCanceled_AfterCreated()
    {
        var ticket = TicketBuilder.New().InCreatedState();
        ticket.Cancel();
        ticket.Status.Should().Be(Status.Canceled);
    }
    
    [Fact]
    public void Status_Should_BeCanceled_AfterAssigned()
    {
        var ticket = TicketBuilder.New().InAssignedState();
        ticket.Cancel();
        ticket.Status.Should().Be(Status.Canceled);
    }
    
    [Fact]
    public void Status_Should_BeCanceled_AfterInProgress()
    {
        var ticket = TicketBuilder.New().InInProgressState(); 
        ticket.Cancel();
        ticket.Status.Should().Be(Status.Canceled);
    }
}