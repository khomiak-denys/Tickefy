using MediatR;

namespace Tickefy.Application.Abstractions.Messaging
{
    /// <summary>
    /// Defines a contract for processing command requests that execute state transitions without returning a data payload.
    /// </summary>
    /// <typeparam name="TCommand">
    /// The specific type of the command message being handled. Must enforce command invariants and implement <see cref="ICommand"/>.
    /// </typeparam>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    { }

    /// <summary>
    /// Defines a contract for processing command requests that execute state transitions and return a resultant data payload or domain result.
    /// </summary>
    /// <typeparam name="TCommand">
    /// The specific type of the command message being handled. Must implement <see cref="ICommand{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response payload returned upon successful command completion. Typically structured as a <see cref="Domain.Common.Results.Result{T}"/> to avoid exception-driven control flow.
    /// </typeparam>
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    { }
}
