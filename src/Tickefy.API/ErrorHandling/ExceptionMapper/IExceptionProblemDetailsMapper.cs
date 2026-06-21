using Microsoft.AspNetCore.Mvc;

namespace Tickefy.API.ErrorHandling.ExceptionMapper
{
    /// <summary>
    /// Maps exceptions to API problem details responses.
    /// </summary>
    public interface IExceptionProblemDetailsMapper
    {
        /// <summary>
        /// Maps an exception to problem details.
        /// </summary>
        /// <param name="exception">The exception to map.</param>
        ProblemDetails Map(Exception exception);
    }
}
