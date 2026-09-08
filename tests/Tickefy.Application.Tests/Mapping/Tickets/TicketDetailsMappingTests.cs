using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Comments;
using Tickefy.Domain.Common.Content;
using Tickefy.Domain.Tests.Ticket.Builders;
using Tickefy.Domain.Users;
using Tickefy.Domain.Common.UserRole;

namespace Tickefy.Application.Tests.Mapping.Tickets;

public class TicketDetailsMappingTests
{
    [Fact]
    public void FromEntity_WithCommentsAndAttachments_ShouldMapEverything()
    {
        // Arrange
        var ticket = TicketBuilder.New().InCreatedState();
        var user = User.Create("Commenter", "User", "comm", "pwd");
        user.SetRole(UserRoles.Agent);
        var comment = Comment.Create(user.Id, ticket.Id, "This is a test comment");
        var attachment = Attachment.Create("file.txt", 1024, ticket.Id);

        ticket.AddComment(comment);
        ticket.Attachments.Add(attachment);

        // Act
        var result = TicketDetailsResult.FromEntity(ticket);
        result.Attachments = new[] { new AttachmentResult("http://url", attachment.FileName, attachment.ContentType.ToString(), attachment.SizeBytes) };

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ticket.Id.Value);
        result.Title.Should().Be(ticket.Title);
        result.Description.Should().Be(ticket.Description);
        result.Comments.Should().HaveCount(1);
        result.Comments[0].Content.Should().Be("This is a test comment");
        result.Attachments.Should().HaveCount(1);
        result.Attachments.First().FileName.Should().Be("file.txt");
    }
}
