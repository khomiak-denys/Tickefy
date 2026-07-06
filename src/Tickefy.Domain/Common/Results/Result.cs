using Tickefy.Domain.Common.Errors;

namespace Tickefy.Domain.Common.Results;

/// <summary>
/// Represents a non-generic operation outcome without a return value payload, implementing standard success and error state tracking.
/// </summary>
public class Result : IResult
{
    /// <inheritdoc />
    public Error Error { get; }

    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public bool IsFailure => !IsSuccess;

    private Result()
    {
        IsSuccess = true;
        Error = null!;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    /// <summary>
    /// Creates a successful operation outcome indicating that the requested action completed without domain errors.
    /// </summary>
    /// <returns>A new <see cref="Result"/> instance where <see cref="IsSuccess"/> is <see langword="true"/>.</returns>
    public static Result Success()
    {
        return new Result();
    }

    /// <summary>
    /// Creates a failed operation outcome encapsulating the specific domain error that caused the termination.
    /// </summary>
    /// <param name="error">
    /// The structured domain error explaining the failure reason. Must not be null or <see cref="Error.None"/>; passing an empty error to a failure result violates domain error invariants.
    /// </param>
    /// <returns>A new <see cref="Result"/> instance where <see cref="IsFailure"/> is <see langword="true"/>.</returns>
    public static Result Failure(Error error)
    {
        return new Result(error);
    }
}

/// <summary>
/// Represents a generic operation outcome containing a return value payload upon success or a structured error upon failure.
/// </summary>
/// <typeparam name="T">The type of the underlying value payload.</typeparam>
public class Result<T> : IResult<T>
{
    /// <inheritdoc />
    public T Value { get; }

    /// <inheritdoc />
    public Error Error { get; }

    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public bool IsFailure => !IsSuccess;


    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null!;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
        Value = default(T)!;
    }

    /// <summary>
    /// Creates a successful generic operation outcome wrapping the provided data payload.
    /// </summary>
    /// <param name="value">
    /// The data payload returned by the operation. Depending on domain requirements, passing null may be valid if <typeparamref name="T"/> is nullable, but callers should ensure value contracts are satisfied.
    /// </param>
    /// <returns>A new <see cref="Result{T}"/> instance where <see cref="IsSuccess"/> is <see langword="true"/> and <see cref="Value"/> is populated.</returns>
    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    /// <summary>
    /// Creates a failed generic operation outcome encapsulating the specific domain error that prevented value generation.
    /// </summary>
    /// <param name="error">
    /// The structured domain error detailing the failure reason. Must represent an actual error state; passing <see cref="Error.None"/> will create an inconsistent failure state.
    /// </param>
    /// <returns>A new <see cref="Result{T}"/> instance where <see cref="IsFailure"/> is <see langword="true"/> and <see cref="Value"/> is uninitialized.</returns>
    public static Result<T> Failure(Error error)
    {
        return new Result<T>(error);
    }

    /// <summary>
    /// Executes one of two branch functions depending on whether the result represents a success or a failure, returning a transformed output.
    /// </summary>
    /// <typeparam name="TOut">The return type produced by both evaluation branches.</typeparam>
    /// <param name="onSuccess">
    /// The transformation function executed when <see cref="IsSuccess"/> is <see langword="true"/>, receiving <see cref="Value"/> as its input. Must not be null.
    /// </param>
    /// <param name="onFailure">
    /// The fallback transformation function executed when <see cref="IsFailure"/> is <see langword="true"/>, receiving <see cref="Error"/> as its input. Must not be null.
    /// </param>
    /// <returns>
    /// The resulting value of type <typeparamref name="TOut"/> produced by evaluating either <paramref name="onSuccess"/> or <paramref name="onFailure"/>.
    /// </returns>
    /// <remarks>
    /// This method enforces exhaustive handling of both successful and failed operation states without resorting to manual conditional checks or exceptions.
    /// </remarks>
    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}
