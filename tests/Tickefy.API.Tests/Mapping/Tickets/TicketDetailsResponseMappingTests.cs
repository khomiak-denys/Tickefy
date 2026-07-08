using FluentAssertions;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Tickets.Common;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Tests.Mapping.Tickets;

public class TicketDetailsResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapEverythingCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var requester = new UserResult(Guid.NewGuid(), "John", "Requester");
        var comment = new CommentResult(Guid.NewGuid(), requester, "Help needed", now);
        var attachment = new AttachmentResult("/path/file.txt", "file.txt", "Document", 1024);
        var action = new TicketActionResult("StartWork", false);

        var result = new TicketDetailsResult(
            id, "Detailed Ticket", "Description here", requester, null, null,
            "IT", "High", "Created", now, now.AddDays(1),
            [comment], [attachment]
        )
        {
            AvailableActions = [action]
        };

        // Act
        var response = TicketDetailsResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
        response.Title.Should().Be("Detailed Ticket");
        response.Comments.Should().HaveCount(1);
        response.Comments[0].Content.Should().Be("Help needed");
        response.Attachments.Should().HaveCount(1);
        response.Attachments[0].FileName.Should().Be("file.txt");
        response.AvailableActions.Should().HaveCount(1);
        response.AvailableActions[0].Key.Should().Be("StartWork");
    }
}
