using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tickefy.API.ErrorHandling.ExceptionMapper;
using Tickefy.Application.Exceptions;

public class ExceptionProblemDetailsMapper : IExceptionProblemDetailsMapper
{
    private readonly Dictionary<Type, Func<Exception, ProblemDetails>> _mappings;

    public ExceptionProblemDetailsMapper()
    {
        _mappings = new()
        {
            {
                typeof(NotFoundException),
                ex => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = ex.Message,
                    Type = "https://httpstatuses.io/404"
                }
            },
            { 
                typeof(AlreadyExistsException),
                ex => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Already exists",
                    Detail = ex.Message,
                    Type = "https://httpstatuses.io/409"
                }
            },
            { 
                typeof(InvalidArgumentException),
                ex => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad request",
                    Detail = ex.Message,
                    Type = "https://httpstatuses.io/400"
                }
            }
        };
    }

    public ProblemDetails Map(Exception exception)
    {
        if (_mappings.TryGetValue(exception.GetType(), out var factory))
            return factory(exception);

        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred",
            Type = "https://httpstatuses.io/500"
        };
    }
}
