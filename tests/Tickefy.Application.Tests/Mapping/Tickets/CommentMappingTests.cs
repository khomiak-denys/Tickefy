using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Comments;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tests.Mapping.Tickets;

public class CommentMappingTests
{
    [Fact]
    public void FromEntity_WhenUserIsLoaded_ShouldMapCorrectly()
    {
        // Arrange
        var user = User.Create("Sam", "Gamgee", "samg", "pwd");
        user.SetRole(UserRoles.Agent);
        var comment = Comment.Create(user.Id, new TicketId(), "Hello World");
        var prop = typeof(Comment).GetProperty(nameof(Comment.User));
        prop?.SetValue(comment, user);

        // Act
        var result = CommentResult.FromEntity(comment);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(comment.Id.Value);
        result.Content.Should().Be("Hello World");
        result.User.Should().NotBeNull();
        result.User.FirstName.Should().Be("Sam");
    }

    [Fact]
    public void FromEntity_WhenUserIsNull_ShouldMapWithUserIdFallback()
    {
        // Arrange
        var userId = new UserId();
        var comment = Comment.Create(userId, new TicketId(), "Fallback comment");

        // Act
        var result = CommentResult.FromEntity(comment);

        // Assert
        result.Should().NotBeNull();
        result.User.Id.Should().Be(userId.Value);
        result.Content.Should().Be("Fallback comment");
    }
}
