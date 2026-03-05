using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tests.Ticket.Builders;

namespace Tickefy.Domain.Tests.Ticket;

public class TicketStatusNegativeTransitionsTests
{
    [Theory]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenDraft(TicketAction action)
    {
        var ticket = TicketBuilder.New().InDraftState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }
    
    [Theory]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenCreated(TicketAction action)
    {
        var ticket = TicketBuilder.New().InCreatedState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenAssigned(TicketAction action)
    {
        var ticket = TicketBuilder.New().InAssignedState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    public void Action_Should_ThrowException_WhenInProgress(TicketAction action)
    {
        var ticket = TicketBuilder.New().InInProgressState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenCompleted(TicketAction action)
    {
        var ticket = TicketBuilder.New().InCompletedState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenReopened(TicketAction action)
    {
        var ticket = TicketBuilder.New().InReopenedState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }

    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenCanceled(TicketAction action)
    {
        var ticket = TicketBuilder.New().InCanceledState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }
    
    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenFailed(TicketAction action)
    {
        var ticket = TicketBuilder.New().InFailedState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }
    
    [Theory]
    [InlineData(TicketAction.Publish)]
    [InlineData(TicketAction.Complete)]
    [InlineData(TicketAction.Cancel)]
    [InlineData(TicketAction.StartWork)]
    [InlineData(TicketAction.Take)]
    [InlineData(TicketAction.Accept)]
    [InlineData(TicketAction.Reopen)]
    [InlineData(TicketAction.Fail)]
    public void Action_Should_ThrowException_WhenAccepted(TicketAction action)
    {
        var ticket = TicketBuilder.New().InAcceptedState();
        var result = Invoke(ticket, action)();
        
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType(typeof(ForbiddenError));
        result.Error.Message.Should().Be("Invalid action");
    }

    private Func<Result> Invoke(Domain.Ticket.Ticket ticket, TicketAction action)
    {
        return action switch
        {
            TicketAction.Cancel => () => ticket.Cancel(),
            TicketAction.Take => () => ticket.Take(new UserId(), new TeamId()),
            TicketAction.StartWork => () => ticket.StartWork(),
            TicketAction.Accept => () => ticket.Accept(),
            TicketAction.Reopen => () => ticket.Reopen(),
            TicketAction.Publish => () => ticket.Publish(string.Empty, string.Empty, new DateTime()),
            TicketAction.Complete => () => ticket.Complete(),
            TicketAction.Fail => () => ticket.Fail(),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
    }
    
}