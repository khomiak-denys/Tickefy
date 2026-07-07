using FluentAssertions;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Tickets.Common;

namespace Tickefy.API.Tests.Mapping.Tickets;

public class TicketActionResponseMappingTests
{
    [Fact]
    public void FromResult_ShouldMapCorrectly()
    {
        // Arrange
        var result = new TicketActionResult("Fail", true);

        // Act
        var response = TicketActionResponse.FromResult(result);

        // Assert
        response.Should().NotBeNull();
        response.Key.Should().Be("Fail");
        response.RequireReason.Should().BeTrue();
    }
}
