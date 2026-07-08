using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tests.Mapping.Teams;

public class TeamDetailsMappingTests
{
    [Fact]
    public void FromEntity_ShouldMapTeamDetailsAndMembersCorrectly()
    {
        // Arrange
        var team = Team.Create("Support", "Support Specialists");
        team.SetCategory(Category.IT);
        team.SetManager(new Domain.Primitives.UserId());

        var member1 = User.Create("Bob", "Agent", "boba", "pwd");
        member1.SetRole(UserRoles.Agent);
        var member2 = User.Create("Charlie", "Agent", "charliea", "pwd");
        member2.SetRole(UserRoles.Agent);
        team.AddMember(member1);
        team.AddMember(member2);

        // Act
        var result = TeamDetailsResult.FromEntity(team);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(team.Id.Value);
        result.Name.Should().Be("Support");
        result.Description.Should().Be("Support Specialists");
        result.Category.Should().Be(Category.IT.ToString());
        result.Members.Should().HaveCount(2);
        result.Members.Select(m => m.Id).Should().Contain([member1.Id.Value, member2.Id.Value]);
    }
}
