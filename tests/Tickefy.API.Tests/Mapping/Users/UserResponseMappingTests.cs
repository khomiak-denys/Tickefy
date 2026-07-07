using FluentAssertions;
using Tickefy.API.User.Responses;
using Tickefy.Application.Teams.Common;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Tests.Mapping.Users;

public class UserResponseMappingTests
{
    [Fact]
    public void FromResult_WithoutTeam_ShouldMapCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var result = new UserDetailsResult(id, "Alice", "Smith", "asmith", "Admin", null, now);

        // Act
        var response = UserResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
        response.FirstName.Should().Be("Alice");
        response.LastName.Should().Be("Smith");
        response.Login.Should().Be("asmith");
        response.Role.Should().Be("Admin");
        response.Team.Should().BeNull();
        response.Created.Should().Be(now);
    }

    [Fact]
    public void FromResult_WithTeam_ShouldMapTeamCorrectly()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerResult = new UserResult(Guid.NewGuid(), "Boss", "Man");
        var teamResult = new TeamResult(teamId, "Core IT", "IT", managerResult);
        var result = new UserDetailsResult(Guid.NewGuid(), "Bob", "Dev", "bdev", "Agent", teamResult, DateTime.UtcNow);

        // Act
        var response = UserResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Team.Should().NotBeNull();
        response.Team!.Id.Should().Be(teamId);
        response.Team.Name.Should().Be("Core IT");
    }
}
