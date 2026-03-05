using Tickefy.Domain.Common.Errors;

namespace Tickefy.Domain.Common.Resultor;

public interface IResult<out T>
{
    public T Value { get; }
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
}

public interface IResult
{
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
}

