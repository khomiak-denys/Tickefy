using AutoMapper;
using Moq;
using Tickefy.Application.Ticket.Common;
using Tickefy.Application.Ticket.GetById;
using Tickefy.Application.User.Common;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tests.Ticket.Builders;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tests.Tickets;

public class GetTicketByIdHandler
{
    [Fact]
    public async Task GetTicketByIdQueryHandler_Should_Return_NotFoundError_On_Null_Ticket()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Tickets.Ticket?)null);

        var mapper = new Mock<IMapper>();
        var handler = new GetTicketByIdQueryHandler(repo.Object, mapper.Object);

        var query = new GetTicketByIdQuery(new UserId(), new List<string>(), new TicketId());
        var result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task GetTicketByIdQueryHandler_Should_Return_ForbiddenError_On_InvalidRoles_()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Tickets.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.Now));
        var mapper = new Mock<IMapper>();

        var handler = new GetTicketByIdQueryHandler(repo.Object, mapper.Object);

        var query = new GetTicketByIdQuery(new UserId(), new List<string>(), new TicketId());
        var result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ForbiddenError>();
    }

    [Fact]
    public async Task GetTicketByIdQueryHandler_Should_DisplayFailOptionForAdmin_When_TicketInInProgressState()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TicketBuilder.New().InInProgressState());
        var mapper = new Mock<IMapper>();

        var mappedDto = new TicketDetailsResult(
            Id: Guid.Empty,
            Title: string.Empty,
            Description: string.Empty,
            Requester: default!,
            AssignedTeam: null,
            AssignedAgent: null,
            Category: string.Empty,
            Priority: string.Empty,
            Status: string.Empty,
            Created: DateTime.UtcNow,
            Deadline: DateTime.UtcNow,
            Comments: new(),
            Attachments: new()
        )
        {
            AvailableActions = Array.Empty<TicketActionResult>()
        };

        mapper.Setup(m => m.Map<TicketDetailsResult>(It.IsAny<Domain.Tickets.Ticket>()))
            .Returns(mappedDto);

        var handler = new GetTicketByIdQueryHandler(repo.Object, mapper.Object);
        var query = new GetTicketByIdQuery(new UserId(), [nameof(UserRoles.Admin)], new TicketId());

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailableActions.Contains(new TicketActionResult(nameof(TicketAction.Fail), true)).Should().BeTrue();
    }

    [Fact]
    public async Task GetTicketByIdQueryHandler_Should_DisplayPublishOptionForRequester_When_TicketInDraftState()
    {
        var userId = new UserId();
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Tickets.Ticket.CreateDraft(string.Empty, string.Empty, userId, DateTime.Now));
        var mapper = new Mock<IMapper>();

        var mappedDto = new TicketDetailsResult(
            Guid.Empty,
            "", "",
            Requester: new UserResult(userId.Value, string.Empty, string.Empty),
            AssignedTeam: null,
            AssignedAgent: null,
            Category: "",
            Priority: "",
            Status: "",
            Created: DateTime.UtcNow,
            Deadline: DateTime.UtcNow,
            Comments: new(),
            Attachments: new()
        )
        {
            AvailableActions = Array.Empty<TicketActionResult>()
        };

        mapper.Setup(m => m.Map<TicketDetailsResult>(It.IsAny<Domain.Tickets.Ticket>()))
            .Returns(mappedDto);

        var handler = new GetTicketByIdQueryHandler(repo.Object, mapper.Object);
        var query = new GetTicketByIdQuery(userId, [nameof(UserRoles.Requester)], new TicketId());

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailableActions.Contains(new TicketActionResult(nameof(TicketAction.Publish), false)).Should().BeTrue();
    }

    [Fact]
    public async Task GetTicketByIdQueryHandler_Should_DisplayStartWorkForAgent_When_TicketInAssignedState()
    {
        var agentId = new UserId();
        var ticket = TicketBuilder.New().InCreatedState();
        ticket.Take(agentId, new TeamId());

        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        var mapper = new Mock<IMapper>();

        var mappedDto = new TicketDetailsResult(
            Guid.Empty,
            "", "",
            Requester: default!,
            AssignedTeam: null,
            AssignedAgent: null,
            Category: "",
            Priority: "",
            Status: "",
            Created: DateTime.UtcNow,
            Deadline: DateTime.UtcNow,
            Comments: new(),
            Attachments: new()
        )
        {
            AvailableActions = Array.Empty<TicketActionResult>()
        };

        mapper.Setup(m => m.Map<TicketDetailsResult>(It.IsAny<Domain.Tickets.Ticket>()))
            .Returns(mappedDto);

        var handler = new GetTicketByIdQueryHandler(repo.Object, mapper.Object);
        var query = new GetTicketByIdQuery(agentId, [nameof(UserRoles.Agent)], new TicketId());

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailableActions.Contains(new TicketActionResult(nameof(TicketAction.StartWork), false)).Should().BeTrue();
    }
}
