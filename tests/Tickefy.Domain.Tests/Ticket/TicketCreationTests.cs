using Tickefy.Domain.Tests.Ticket.Builders;

namespace Tickefy.Domain.Tests.Ticket;

public class TicketCreationTests
{
    [Fact]
    public void Status_ShouldBe_Draft_WhenCreatingDraft()
    {
        var ticket = TicketBuilder.New().InDraftState();
        ticket.Status.Should().Be(Status.Draft);
    }

    [Fact]
    public void Status_Should_BeCreated_WhenCreating()
    {
        var ticket = TicketBuilder.New().InCreatedState();
        ticket.Status.Should().Be(Status.Created);
    }
}