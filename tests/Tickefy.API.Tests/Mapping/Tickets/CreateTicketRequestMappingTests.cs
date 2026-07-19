using FluentAssertions;
using Tickefy.API.Attachments.Requests;
using Tickefy.API.Ticket.Requests;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Tests.Mapping.Tickets;

public class CreateTicketRequestMappingTests
{
    [Fact]
    public void ToCommand_ShouldMapCorrectly()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var deadline = DateTime.UtcNow.AddDays(7);
        var request = new CreateTicketRequest
        {
            Title = "Test Ticket",
            Description = "Test Description",
            Deadline = deadline,
            UploadFiles = new List<UploadFileRequest>
            {
                new() { ClientFileId = "file1", FileName = "test.png", SizeBytes = 2048 },
                new() { ClientFileId = "file2", FileName = "notes.txt", SizeBytes = 1024 }
            }
        };

        // Act
        var command = request.ToCommand(userId);

        // Assert
        command.Should().NotBeNull();
        command.UserId.Should().Be(userId);
        command.Title.Should().Be(request.Title);
        command.Description.Should().Be(request.Description);
        command.Deadline.Should().Be(request.Deadline);
        command.Files.Should().HaveCount(2);

        command.Files[0].ClientFileId.Should().Be("file1");
        command.Files[0].FileName.Should().Be("test.png");
        command.Files[0].SizeBytes.Should().Be(2048);

        command.Files[1].ClientFileId.Should().Be("file2");
        command.Files[1].FileName.Should().Be("notes.txt");
        command.Files[1].SizeBytes.Should().Be(1024);
    }
}
