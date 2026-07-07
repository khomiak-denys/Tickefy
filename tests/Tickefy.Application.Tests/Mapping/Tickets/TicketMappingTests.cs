using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Tests.Ticket.Builders;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tests.Mapping.Tickets;

public class TicketMappingTests
{
    [Fact]
    public void FromEntity_WithoutAssignedTeamAndAgent_ShouldMapCorrectly()
    {
        // Arrange
        var ticket = TicketBuilder.New().InCreatedState();

        // Act
        var result = TicketResult.FromEntity(ticket);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ticket.Id.Value);
        result.Title.Should().Be(ticket.Title);
        result.Status.Should().Be(ticket.Status.ToString());
        result.AssignedTeam.Should().BeNull();
        result.AssignedAgent.Should().BeNull();
    }

    [Fact]
    public void FromEntity_WithAssignedTeamAndAgent_ShouldMapCorrectly()
    {
        // Arrange
        var ticket = TicketBuilder.New().InCreatedState();
        var agent = User.Create("Test", "Agent", "agent", "pwd");
        agent.SetRole(UserRoles.Agent);
        var team = Team.Create("Helpdesk", null);
        
        ticket.Take(agent.Id, team.Id);
        typeof(Domain.Tickets.Ticket).GetProperty(nameof(Domain.Tickets.Ticket.AssignedTeam))?.SetValue(ticket, team);
        typeof(Domain.Tickets.Ticket).GetProperty(nameof(Domain.Tickets.Ticket.AssignedAgent))?.SetValue(ticket, agent);

        // Act
        var result = TicketResult.FromEntity(ticket);

        // Assert
        result.Should().NotBeNull();
        result.AssignedTeam.Should().NotBeNull();
        result.AssignedTeam!.Id.Should().Be(team.Id.Value);
        result.AssignedAgent.Should().NotBeNull();
        result.AssignedAgent!.Id.Should().Be(agent.Id.Value);
    }
}
