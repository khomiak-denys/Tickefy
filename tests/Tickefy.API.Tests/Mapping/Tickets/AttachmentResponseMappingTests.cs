using FluentAssertions;
using Tickefy.API.Attachments.Response;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Attachments.Upload;
using Tickefy.Application.Tickets.Common;

namespace Tickefy.API.Tests.Mapping.Tickets;

public class AttachmentResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapCorrectly()
    {
        // Arrange
        var result = new AttachmentResult("https://uploads/photo.png", "photo.png", "Photo", 4096);

        // Act
        var response = AttachmentResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Url.Should().Be("https://uploads/photo.png");
        response.FileName.Should().Be("photo.png");
        response.ContentType.Should().Be("Photo");
        response.SizeBytes.Should().Be(4096);
    }

    [Fact]
    public void FromResult_ForUploadResult_ShouldMapCorrectly()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var result = new AttachmentUploadResult("client-file-id", attachmentId, "photo", "https://storage.fake/upload");

        // Act
        var response = AttachmentUploadResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.ClientFileId.Should().Be("client-file-id");
        response.AttachmentId.Should().Be(attachmentId);
        response.FileName.Should().Be("photo");
        response.UploadUrl.Should().Be("https://storage.fake/upload");
    }
}
