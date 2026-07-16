using Microsoft.Extensions.Logging;
using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.AI.Dtos;
using Tickefy.Application.Tickets.Publish;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tests.Tickets;

public class PublishTicketHandlerTests
{
    [Fact]
    public async Task PublishTicketCommandHandler_Should_Return_NotFoundError_When_Ticket_Is_NotFound()
    {
        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Tickets.Ticket?)null);

        var command = new PublishTicketCommand
        {
            UserId = new UserId(),
            Roles = [string.Empty],
            TicketId = new TicketId(),
            Title = string.Empty,
            Description = string.Empty,
            Deadline = new DateTime()
        };

        var activityLogRepository = new Mock<IActivityLogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var aiService = new Mock<IAiService>();
        var aiResponseParser = new Mock<IAiResponseParser>();
        var logger = new Mock<ILogger<PublishTicketCommandHandler>>();

        var handler = new PublishTicketCommandHandler(
            ticketRepository.Object,
            activityLogRepository.Object,
            unitOfWork.Object,
            aiService.Object,
            aiResponseParser.Object,
            logger.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PublishTicketCommandHandler_Should_Return_ForbiddenError_When_UserIsNotRequester_OfTheTicket()
    {
        var userId = new UserId();
        var ticket = Domain.Tickets.Ticket.CreateDraft(string.Empty, string.Empty, userId, new DateTime());

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var command = new PublishTicketCommand
        {
            UserId = new UserId(),
            Roles = [nameof(UserRoles.Admin), nameof(UserRoles.Agent)],
            TicketId = new TicketId(),
            Title = string.Empty,
            Description = string.Empty,
            Deadline = new DateTime()
        };

        var activityLogRepository = new Mock<IActivityLogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var aiService = new Mock<IAiService>();
        var aiResponseParser = new Mock<IAiResponseParser>();
        var logger = new Mock<ILogger<PublishTicketCommandHandler>>();

        var handler = new PublishTicketCommandHandler(
            ticketRepository.Object,
            activityLogRepository.Object,
            unitOfWork.Object,
            aiService.Object,
            aiResponseParser.Object,
            logger.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ForbiddenError>();
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PublishTicketCommandHandler_Should_SetDefaultsPriorityAndCategory_When_AiServicesInNotWorking()
    {
        var userId = new UserId();
        var ticket = Domain.Tickets.Ticket.CreateDraft(string.Empty, string.Empty, userId, new DateTime());

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var activityLogRepository = new Mock<IActivityLogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var aiService = new Mock<IAiService>();
        var aiResponseParser = new Mock<IAiResponseParser>();
        var logger = new Mock<ILogger<PublishTicketCommandHandler>>();

        aiService.Setup(ai => ai.AnalyzeTicketAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("AI service failed"));

        var command = new PublishTicketCommand
        {
            UserId = userId,
            Roles = [nameof(UserRoles.Requester)],
            TicketId = ticket.Id,
            Title = string.Empty,
            Description = string.Empty,
            Deadline = new DateTime()
        };

        var handler = new PublishTicketCommandHandler(
            ticketRepository.Object,
            activityLogRepository.Object,
            unitOfWork.Object,
            aiService.Object,
            aiResponseParser.Object,
            logger.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        ticket.Priority.Should().Be(Priority.Medium);
        ticket.Category.Should().Be(Category.Other);

        activityLogRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLogs.ActivityLog>(log =>
            log.TicketId == ticket.Id &&
            log.UserId == userId &&
            log.EventType == EventType.StatusChanged &&
            log.Description.Contains("Ticket published."))));

        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishTicketCommandHandler_Should_SetPriorityAndCategory_When_AiServicesInWorking()
    {
        var userId = new UserId();
        var ticket = Domain.Tickets.Ticket.CreateDraft(string.Empty, string.Empty, userId, new DateTime());

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var command = new PublishTicketCommand
        {
            UserId = userId,
            Roles = [nameof(UserRoles.Requester)],
            TicketId = ticket.Id,
            Title = string.Empty,
            Description = string.Empty,
            Deadline = new DateTime()
        };

        var activityLogRepository = new Mock<IActivityLogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var aiService = new Mock<IAiService>();
        var aiResponseParser = new Mock<IAiResponseParser>();
        var logger = new Mock<ILogger<PublishTicketCommandHandler>>();

        aiResponseParser.Setup(parser => parser.ParseCategory(It.IsAny<AiResponse>()))
            .Returns(Category.Design);
        aiResponseParser.Setup(parser => parser.ParsePriority(It.IsAny<AiResponse>()))
            .Returns(Priority.High);

        var handler = new PublishTicketCommandHandler(
            ticketRepository.Object,
            activityLogRepository.Object,
            unitOfWork.Object,
            aiService.Object,
            aiResponseParser.Object,
            logger.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        ticket.Priority.Should().Be(Priority.High);
        ticket.Category.Should().Be(Category.Design);

        activityLogRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLogs.ActivityLog>(log =>
            log.TicketId == ticket.Id &&
            log.UserId == userId &&
            log.EventType == EventType.StatusChanged &&
            log.Description.Contains("Ticket published."))));

        unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
