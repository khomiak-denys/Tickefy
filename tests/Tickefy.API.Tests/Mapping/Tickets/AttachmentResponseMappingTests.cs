using FluentAssertions;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Tickets.Common;

namespace Tickefy.API.Tests.Mapping.Tickets;

public class AttachmentResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapCorrectly()
    {
        // Arrange
        var result = new AttachmentResult("/uploads/photo.png", "photo.png", "Photo", 4096);

        // Act
        var response = AttachmentResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.FilePath.Should().Be("/uploads/photo.png");
        response.FileName.Should().Be("photo.png");
        response.ContentType.Should().Be("Photo");
        response.SizeBytes.Should().Be(4096);
    }
}
