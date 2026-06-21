using MediatR;

namespace Tickefy.Application.Abstractions.Messaging
{
    /// <summary>
    /// Handles a command that does not return a response.
    /// </summary>
    /// <typeparam name="TCommand">The command type to handle.</typeparam>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    { }

    /// <summary>
    /// Handles a command and returns a response.
    /// </summary>
    /// <typeparam name="TCommand">The command type to handle.</typeparam>
    /// <typeparam name="TResponse">The response type returned by the command.</typeparam>
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    { }
}
