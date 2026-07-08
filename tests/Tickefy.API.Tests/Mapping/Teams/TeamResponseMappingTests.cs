using FluentAssertions;
using Tickefy.API.Team.Responses;
using Tickefy.Application.Teams.Common;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Tests.Mapping.Teams;

public class TeamResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapCorrectly()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerResult = new UserResult(Guid.NewGuid(), "Alice", "Admin");
        var result = new TeamResult(teamId, "Core IT", "IT", managerResult);

        // Act
        var response = TeamResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(teamId);
        response.Name.Should().Be("Core IT");
        response.Category.Should().Be("IT");
        response.Manager.Should().NotBeNull();
        response.Manager.FirstName.Should().Be("Alice");
    }
}
