using FluentAssertions;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Tickets.Common;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Tests.Mapping.Tickets;

public class CommentResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var user = new UserResult(Guid.NewGuid(), "Jane", "Doe");
        var result = new CommentResult(id, user, "Test Comment", now);

        // Act
        var response = CommentResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
        response.User.FirstName.Should().Be("Jane");
        response.Content.Should().Be("Test Comment");
        response.Created.Should().Be(now);
    }
}
