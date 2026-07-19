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
        var attachment = Attachment.Create("test.png", ContentType.Photo, 2048, ticketId);

        // Act
        var result = AttachmentResult.FromEntity(attachment);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be("test.png");
        result.FilePath.Should().Be($"tickets/{ticketId.Value}/attachments/{attachment.Id.Value}");
        result.ContentType.Should().Be(ContentType.Photo.ToString());
        result.SizeBytes.Should().Be(2048);
    }

    [Theory]
    [InlineData(ContentType.Photo, "Photo")]
    [InlineData(ContentType.Document, "Document")]
    [InlineData(ContentType.Video, "Video")]
    [InlineData(ContentType.Archive, "Archive")]
    public void FromEntity_ShouldMapAllContentTypes(ContentType contentType, string expectedString)
    {
        // Arrange
        var ticketId = new TicketId();
        var attachment = Attachment.Create("test", contentType, 1024, ticketId);

        // Act
        var result = AttachmentResult.FromEntity(attachment);

        // Assert
        result.ContentType.Should().Be(expectedString);
    }
}
