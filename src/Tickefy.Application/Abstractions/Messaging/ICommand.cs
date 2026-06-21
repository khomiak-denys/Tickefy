using MediatR;

namespace Tickefy.Application.Abstractions.Messaging
{
    /// <summary>
    /// Represents a command that does not return a response.
    /// </summary>
    public interface ICommand : IRequest { }

    /// <summary>
    /// Represents a command that returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The response type returned by the command.</typeparam>
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
