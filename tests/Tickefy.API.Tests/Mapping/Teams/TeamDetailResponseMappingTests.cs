using FluentAssertions;
using Tickefy.API.Team.Responses;
using Tickefy.Application.Teams.Common;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Tests.Mapping.Teams;

public class TeamDetailResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapTeamDetailsAndMembersCorrectly()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var managerResult = new UserResult(Guid.NewGuid(), "Boss", "Manager");
        var member1 = new UserResult(Guid.NewGuid(), "Bob", "Dev");
        var member2 = new UserResult(Guid.NewGuid(), "Charlie", "QA");
        var result = new TeamDetailsResult(teamId, "Core IT", "IT Infrastructure", "IT", managerResult, [member1, member2]);

        // Act
        var response = TeamDetailResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(teamId);
        response.Name.Should().Be("Core IT");
        response.Description.Should().Be("IT Infrastructure");
        response.Category.Should().Be("IT");
        response.Manager.FirstName.Should().Be("Boss");
        response.Members.Should().HaveCount(2);
        response.Members.Select(m => m.FirstName).Should().Contain(["Bob", "Charlie"]);
    }
}
