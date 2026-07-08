using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tests.Mapping.Users;

public class UserMappingTests
{
    [Fact]
    public void FromEntity_WhenUserIsNotNull_ShouldMapCorrectly()
    {
        // Arrange
        var user = User.Create("John", "Doe", "johndoe", "hash_secret");

        // Act
        var result = UserResult.FromEntity(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id.Value);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Fact]
    public void FromEntity_WhenUserIsNull_ShouldReturnEmptyResult()
    {
        // Act
        var result = UserResult.FromEntity(null);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(Guid.Empty);
        result.FirstName.Should().BeEmpty();
        result.LastName.Should().BeEmpty();
    }
}
