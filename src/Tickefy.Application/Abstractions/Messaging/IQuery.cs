using MediatR;

namespace Tickefy.Application.Abstractions.Messaging
{
    /// <summary>
    /// Represents a query that returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The response type returned by the query.</typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
