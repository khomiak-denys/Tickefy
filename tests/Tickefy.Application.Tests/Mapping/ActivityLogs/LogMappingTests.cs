using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tests.Mapping.ActivityLogs;

public class LogMappingTests
{
    [Fact]
    public void FromEntity_WhenUserIsLoaded_ShouldMapCorrectly()
    {
        // Arrange
        var user = User.Create("Log", "User", "logu", "pwd");
        user.SetRole(UserRoles.Admin);
        var ticketId = new TicketId();
        var log = ActivityLog.Create(ticketId, user.Id, EventType.RequestCreated, "Ticket was created");
        var prop = typeof(ActivityLog).GetProperty(nameof(ActivityLog.User));
        prop?.SetValue(log, user);

        // Act
        var result = LogResult.FromEntity(log);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(log.Id.Value);
        result.TicketId.Should().Be(ticketId.Value);
        result.EventType.Should().Be(EventType.RequestCreated.ToString());
        result.Description.Should().Be("Ticket was created");
        result.User.Should().NotBeNull();
        result.User.FirstName.Should().Be("Log");
    }

    [Fact]
    public void FromEntity_WhenUserIsNull_ShouldMapWithUserIdFallback()
    {
        // Arrange
        var userId = new UserId();
        var ticketId = new TicketId();
        var log = ActivityLog.Create(ticketId, userId, EventType.StatusChanged, "Status changed to InProgress");

        // Act
        var result = LogResult.FromEntity(log);

        // Assert
        result.Should().NotBeNull();
        result.User.Id.Should().Be(userId.Value);
        result.EventType.Should().Be(EventType.StatusChanged.ToString());
    }
}
