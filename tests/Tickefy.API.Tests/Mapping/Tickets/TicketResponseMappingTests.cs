using FluentAssertions;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Teams.Common;
using Tickefy.Application.Tickets.Common;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Tests.Mapping.Tickets;

public class TicketResponseMappingTests
{
    [Fact]
    public void FromResult_WithoutAssignedTeamAndAgent_ShouldMapCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var requester = new UserResult(Guid.NewGuid(), "John", "Requester");
        var result = new TicketResult(id, "Test Ticket", requester, null, null, "IT", "High", "Created", now);

        // Act
        var response = TicketResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
        response.Title.Should().Be("Test Ticket");
        response.Requester.FirstName.Should().Be("John");
        response.AssignedTeam.Should().BeNull();
        response.AssignedAgent.Should().BeNull();
    }

    [Fact]
    public void FromResult_WithAssignedTeamAndAgent_ShouldMapCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var requester = new UserResult(Guid.NewGuid(), "John", "Requester");
        var agent = new UserResult(Guid.NewGuid(), "Agent", "Smith");
        var team = new TeamResult(Guid.NewGuid(), "Support", "IT", new UserResult(Guid.NewGuid(), "Manager", "One"));
        var result = new TicketResult(id, "Assigned Ticket", requester, team, agent, "IT", "Low", "Assigned", DateTime.UtcNow);

        // Act
        var response = TicketResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.AssignedTeam.Should().NotBeNull();
        response.AssignedTeam!.Name.Should().Be("Support");
        response.AssignedAgent.Should().NotBeNull();
        response.AssignedAgent!.FirstName.Should().Be("Agent");
    }
}
