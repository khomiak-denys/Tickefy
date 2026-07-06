using Tickefy.Domain.Common.Errors;

namespace Tickefy.Domain.Common.Results;

/// <summary>
/// Defines a contract representing the deterministic execution outcome of an operation without a return payload, encapsulating success status and detailed error context.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets the structured domain error detailing why the operation failed.
    /// </summary>
    /// <remarks>
    /// This property is expected to be <see cref="Error.None"/> or <see langword="null"/> when <see cref="IsSuccess"/> is <see langword="true"/>. Attempting to access or evaluate this error during a successful outcome should be avoided.
    /// </remarks>
    public Error Error { get; }

    /// <summary>
    /// Gets a value indicating whether the operation executed without encountering any domain or validation failures.
    /// </summary>
    /// <remarks>
    /// When <see langword="true"/>, callers may proceed under the assumption that state changes occurred as expected and no error occurred.
    /// </remarks>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation terminated abnormally or failed business rule validation.
    /// </summary>
    /// <remarks>
    /// This is strictly the logical inverse of <see cref="IsSuccess"/>. When <see langword="true"/>, callers must inspect <see cref="Error"/> to determine the failure cause and take appropriate corrective or reporting action.
    /// </remarks>
    public bool IsFailure { get; }
}

/// <summary>
/// Defines a contract representing the deterministic execution outcome of an operation that produces a value payload upon success.
/// </summary>
/// <typeparam name="T">
/// The specific type of the data payload returned by the operation when executed successfully.
/// </typeparam>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the underlying data payload produced by a successful operation execution.
    /// </summary>
    /// <remarks>
    /// Caution: Accessing this property when <see cref="IResult.IsFailure"/> is <see langword="true"/> may return the default value of <typeparamref name="T"/> or throw an invalid operation exception depending on implementation. Always verify <see cref="IResult.IsSuccess"/> before reading this value.
    /// </remarks>
    public T Value { get; }
}



