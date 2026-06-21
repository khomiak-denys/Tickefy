using Tickefy.Domain.Common.Errors;

namespace Tickefy.Domain.Common.Results;

/// <summary>
/// Represents the outcome of an operation.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets the error associated with a failed result.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure { get; }
}

/// <summary>
/// Represents the outcome of an operation that returns a value.
/// </summary>
/// <typeparam name="T">The value type returned by the operation.</typeparam>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the value returned by a successful result.
    /// </summary>
    public T Value { get; }
}



