namespace Tickefy.Domain.Tests.Ticket;

public class TicketStatusTransitions
{
    [Fact]
    public void Status_ShouldBe_Draft_WhenCreatingDraft()
    {
        var ticket = Domain.Ticket.Ticket.CreateDraft("some title", "some description", new UserId(), new DateTime());
        ticket.Status.Should().Be(Status.Draft);
    }

    [Fact]
    public void Status_Should_BeCreated_WhenCreating()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Status.Should().Be(Status.Created);
    }

    [Fact]
    public void Status_Should_BeCreated_AfterDraft()
    {
        var ticket = Domain.Ticket.Ticket.CreateDraft("some title", "some description", new UserId(), new DateTime());
        ticket.Publish();
        ticket.Status.Should().Be(Status.Created);
    }

    [Fact]
    public void Status_Should_BeAssigned_AfterCreated()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.Status.Should().Be(Status.Assigned);
    }

    [Fact]
    public void Status_Should_BeInProgress_AfterAssigned()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Status.Should().Be(Status.InProgress);
    }

    [Fact]
    public void Status_Should_BeCompleted_AfterInProgress()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Complete();
        ticket.Status.Should().Be(Status.Completed);
    }

    [Fact]
    public void Status_Should_BeReopened_AfterCompleted()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Complete();
        ticket.Reopen();
        ticket.Status.Should().Be(Status.Reopened);
    }

    [Fact]
    public void Status_Should_BeAccepted_AfterCompleted()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Complete();
        ticket.Accept();
        ticket.Status.Should().Be(Status.Accepted);
    }

    [Fact]
    public void Status_Should_BeFailed_AfterInProgress()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Fail();
        ticket.Status.Should().Be(Status.Failed);
    }

    [Fact]
    public void Status_Should_BeCancelled_AfterCreated()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Cancel();
        ticket.Status.Should().Be(Status.Canceled);
    }
    
    [Fact]
    public void Status_Should_BeCancelled_AfterAssigned()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.Cancel();
        ticket.Status.Should().Be(Status.Canceled);
    }
    
    [Fact]
    public void Status_Should_BeCancelled_AfterInProgress()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork(); 
        ticket.Cancel();
        ticket.Status.Should().Be(Status.Canceled);
    }
    
    
}