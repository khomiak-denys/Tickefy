using FluentAssertions;
using Tickefy.API.User.Responses;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Tests.Mapping.Users;

public class MinimalUserResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var result = new UserResult(id, "John", "Doe");

        // Act
        var response = MinimalUserResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
        response.FirstName.Should().Be("John");
        response.LastName.Should().Be("Doe");
    }
}
