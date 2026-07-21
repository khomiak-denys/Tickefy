using MediatR;

namespace Tickefy.Application.Abstractions.Messaging
{
    /// <summary>
    /// Defines a contract for processing read-only query requests, evaluating system state without executing side effects or persistence mutations.
    /// </summary>
    /// <typeparam name="TQuery">
    /// The specific type of the query message being handled. Must enforce read-only semantics and implement <see cref="IQuery{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The return payload type produced by evaluating the query. Typically structured as a <see cref="Tickefy.Domain.Common.Results.Result{TResponse}"/> containing view models or DTO collections.
    /// </typeparam>
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        new Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
    }
}
