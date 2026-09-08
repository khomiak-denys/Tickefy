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
        var attachment = Attachment.Create("test.png", 2048, ticketId);
        var url = $"http://storage/{attachment.FilePath}";

        // Act
        var result = new AttachmentResult(url, attachment.FileName, attachment.ContentType.ToString(), attachment.SizeBytes);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be("test.png");
        result.Url.Should().Be(url);
        result.ContentType.Should().Be(ContentType.Photo.ToString());
        result.SizeBytes.Should().Be(2048);
    }

    [Theory]
    [InlineData("test.png", ContentType.Photo, "Photo")]
    [InlineData("doc.pdf", ContentType.Document, "Document")]
    [InlineData("video.mp4", ContentType.Video, "Video")]
    [InlineData("archive.zip", ContentType.Archive, "Archive")]
    public void Attachment_ShouldDetectContentTypeFromExtension(string fileName, ContentType expectedContentType, string expectedString)
    {
        // Arrange
        var ticketId = new TicketId();
        var attachment = Attachment.Create(fileName, 1024, ticketId);

        // Assert
        attachment.ContentType.Should().Be(expectedContentType);
        attachment.ContentType.ToString().Should().Be(expectedString);
    }
}
