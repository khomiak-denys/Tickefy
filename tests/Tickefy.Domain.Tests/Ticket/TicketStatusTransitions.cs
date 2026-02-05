namespace Tickefy.Domain.Tests.Ticket;

public class TicketStatusTransitions
{
    [Fact]
    public void ShouldBeDraft()
    {
        var ticket = Domain.Ticket.Ticket.CreateDraft("some title", "some description", new UserId(), new DateTime());
        ticket.Status.Should().Be(Status.Draft);
    }

    [Fact]
    public void ShouldBeCreated()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Status.Should().Be(Status.Created);
    }

    [Fact]
    public void ShouldBePublished()
    {
        var ticket = Domain.Ticket.Ticket.CreateDraft("some title", "some description", new UserId(), new DateTime());
        ticket.Publish();
        ticket.Status.Should().Be(Status.Created);
    }

    [Fact]
    public void ShouldBeAssigned()
    {
        var ticket = Domain.Ticket.Ticket.Create("some title", "some description", new UserId(), new DateTime());
        ticket.Take(new UserId(), new TeamId());
        ticket.Status.Should().Be(Status.Assigned);
    }
    
    
}