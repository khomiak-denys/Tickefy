using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tests.Mapping.Teams;

public class TeamMappingTests
{
    [Fact]
    public void FromEntity_WhenManagerIsLoaded_ShouldMapCorrectly()
    {
        // Arrange
        var manager = User.Create("Alice", "Manager", "alicem", "hash");
        manager.SetRole(UserRoles.Admin);
        var team = Team.Create("Core Tech", "Core technical team");
        team.SetCategory(Category.IT);
        team.SetManager(manager.Id);
        var prop = typeof(Team).GetProperty(nameof(Team.Manager));
        prop?.SetValue(team, manager);

        // Act
        var result = TeamResult.FromEntity(team);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(team.Id.Value);
        result.Name.Should().Be("Core Tech");
        result.Category.Should().Be(Category.IT.ToString());
        result.Manager.Should().NotBeNull();
    }

    [Fact]
    public void FromEntity_WhenManagerIsNull_ShouldMapWithManagerIdFallback()
    {
        // Arrange
        var managerId = new Domain.Primitives.UserId();
        var team = Team.Create("Core Tech", "Core technical team");
        team.SetCategory(Category.Finance);
        team.SetManager(managerId);

        // Act
        var result = TeamResult.FromEntity(team);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(team.Id.Value);
        result.Category.Should().Be(Category.Finance.ToString());
        result.Manager.Id.Should().Be(managerId.Value);
        result.Manager.FirstName.Should().BeEmpty();
    }
}
