using FluentAssertions;
using Tickefy.API.Auth.Responses;
using Tickefy.Application.Auth.Common;

namespace Tickefy.API.Tests.Mapping.Auth;

public class LoginResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var result = new LoginResult(id, "John", "Doe", "jdoe", "jwt_token_here", "refresh_token_here");

        // Act
        var response = LoginResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
        response.FirstName.Should().Be("John");
        response.LastName.Should().Be("Doe");
        response.Login.Should().Be("jdoe");
        response.Token.Should().Be("jwt_token_here");
    }
}
