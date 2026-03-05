using Tickefy.Domain.Common.Errors;

namespace Tickefy.Domain.Common.Results;

public interface IResult<out T> : IResult
{
    public T Value { get; }
}

public interface IResult
{
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
}

