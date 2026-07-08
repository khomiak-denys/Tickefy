using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Common.Content;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tests.Mapping.Tickets;

public class AttachmentMappingTests
{
    [Fact]
    public void FromEntity_ShouldMapCorrectly()
    {
        // Arrange
        var ticketId = new TicketId();
        var attachment = Attachment.Create("/uploads/test.png", "test.png", ContentType.Photo, 2048, ticketId);

        // Act
        var result = AttachmentResult.FromEntity(attachment);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be("test.png");
        result.FilePath.Should().Be("/uploads/test.png");
        result.ContentType.Should().Be(ContentType.Photo.ToString());
        result.SizeBytes.Should().Be(2048);
    }
}
