using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tests.Mapping.Users;

public class UserDetailsMappingTests
{
    [Fact]
    public void FromEntity_WithoutTeam_ShouldMapCorrectly()
    {
        // Arrange
        var user = User.Create("Jane", "Smith", "janesmith", "hashed");
        user.SetRole(UserRoles.Agent);

        // Act
        var result = UserDetailsResult.FromEntity(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id.Value);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Login.Should().Be("janesmith");
        result.Role.Should().Be(UserRoles.Agent.ToString());
        result.Team.Should().BeNull();
        result.Created.Should().Be(user.Created);
    }

    [Fact]
    public void FromEntity_WithTeam_ShouldMapTeamCorrectly()
    {
        // Arrange
        var manager = User.Create("Boss", "Man", "boss", "pwd");
        manager.SetRole(UserRoles.Admin);
        var team = Team.Create("Dev Team", "Developers");
        team.SetManager(manager.Id);

        var user = User.Create("Dev", "Guy", "devguy", "pwd");
        user.SetRole(UserRoles.Agent);
        user.SetTeam(team);

        // Act
        var result = UserDetailsResult.FromEntity(user);

        // Assert
        result.Should().NotBeNull();
        result.Team.Should().NotBeNull();
        result.Team!.Id.Should().Be(team.Id.Value);
        result.Team.Name.Should().Be("Dev Team");
    }
}
