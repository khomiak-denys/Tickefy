using FluentAssertions;
using Tickefy.API.ActivityLog.Responses;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Tests.Mapping.ActivityLogs;

public class LogResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var user = new UserResult(Guid.NewGuid(), "Log", "User");
        var result = new LogResult(id, ticketId, user, "RequestCreated", "Ticket created", now);

        // Act
        var response = LogResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
        response.TicketId.Should().Be(ticketId);
        response.User.FirstName.Should().Be("Log");
        response.EventType.Should().Be("RequestCreated");
        response.Description.Should().Be("Ticket created");
        response.Created.Should().Be(now);
    }
}
