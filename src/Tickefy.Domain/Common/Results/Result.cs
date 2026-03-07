using Tickefy.Domain.Common.Errors;

namespace Tickefy.Domain.Common.Results;

public class Result : IResult
{
    public Error Error { get; }
    public bool IsSuccess { get; }
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

    public static Result Success()
    {
        return new Result();
    }

    public static Result Failure(Error error)
    {
        return new Result(error);
    }

    public static implicit operator Result(Error error)
    {
        return new Result(error);   
    }
}

public class Result<T> : IResult<T>
{
    public T Value { get; }
    public Error Error { get; }
    public bool IsSuccess { get; }
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

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Error error)
    {
        return new Result<T>(error);
    }

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
    {
        return IsSuccess ? onSuccess(Value) :  onFailure(Error);
    }
}