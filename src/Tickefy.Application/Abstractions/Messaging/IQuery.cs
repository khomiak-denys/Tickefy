using MediatR;

namespace Tickefy.Application.Abstractions.Messaging
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}