using MediatR;

namespace Tickefy.Application.Abstractions.Messaging
{
    /// <summary>
    /// Represents a read-only request that retrieves system state without executing side effects or mutations, in accordance with CQRS principles.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The data transfer object or domain view model returned by the query evaluation.
    /// </typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
