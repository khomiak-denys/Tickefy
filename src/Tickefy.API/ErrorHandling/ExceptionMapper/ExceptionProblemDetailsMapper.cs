using Microsoft.AspNetCore.Mvc;

namespace Tickefy.API.ErrorHandling.ExceptionMapper
{
    public class ExceptionProblemDetailsMapper : IExceptionProblemDetailsMapper
    {
        private readonly Dictionary<Type, Func<Exception, ProblemDetails>> _mappings;

        public ExceptionProblemDetailsMapper()
        {
            _mappings = new()
        {
            {
                typeof(FluentValidation.ValidationException),
                ex =>
                {
                    var validationException = (FluentValidation.ValidationException)ex;

                    return new ValidationProblemDetails(
                        validationException.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray()
                            )
                    )
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validation Error",
                        Type = "https://httpstatuses.io/400"
                    };
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
}