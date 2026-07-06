using MediatR;

namespace Tickefy.Application.Abstractions.Messaging
{
    /// <summary>
    /// Represents a transactional request that mutates system state without returning a payload, in accordance with CQRS principles.
    /// </summary>
    public interface ICommand : IRequest { }

    /// <summary>
    /// Represents a transactional request that mutates system state and returns a response payload, in accordance with CQRS principles.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of data or outcome model returned upon command completion. Callers should expect domain results encapsulating success or validation failure states.
    /// </typeparam>
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
