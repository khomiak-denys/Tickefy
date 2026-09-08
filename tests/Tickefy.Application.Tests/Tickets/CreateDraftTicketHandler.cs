using Microsoft.Extensions.Logging;
using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Tickets.CreateDraft;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tests.Tickets;

public class CreateDraftTicketHandlerTests
{
    [Fact]
    public async Task CreateDraftTicketHandler_Should_Successfully_Create_Draft_Ticket()
    {
        var userId = new UserId();
        var title = string.Empty;
        var description = string.Empty;
        var deadline = new DateTime(2026, 12, 31);
        var ticketRepository = new Mock<ITicketRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<CreateDraftTicketCommandHandler>>();

        var handler = new CreateDraftTicketCommandHandler(
            ticketRepository.Object,
            unitOfWork.Object,
            logger.Object);

        var command = new CreateDraftTicketCommand
        {
            UserId = userId,
            Title = title,
            Description = description,
            Deadline = deadline
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        ticketRepository.Verify(repo => repo.Add(It.Is<Domain.Tickets.Ticket>(t =>
            t.Title == title &&
            t.Description == description &&
            t.Deadline == deadline &&
            t.RequesterId == userId)));

        unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
