using Microsoft.AspNetCore.Mvc;

namespace Tickefy.API.ErrorHandling.ExceptionMapper
{
    /// <summary>
    /// Defines contract requirements for translating unhandled runtime exceptions into standardized HTTP RFC 7807 problem details responses.
    /// </summary>
    public interface IExceptionProblemDetailsMapper
    {
        /// <summary>
        /// Evaluates a runtime exception and constructs a formatted HTTP problem details object containing HTTP status code, title, and validation error dictionaries where applicable.
        /// </summary>
        /// <param name="exception">
        /// The caught runtime exception instance to map. Must not be null; passing null will cause null reference evaluation errors during dictionary lookup or casting.
        /// </param>
        /// <returns>
        /// A populated <see cref="ProblemDetails"/> or <see cref="ValidationProblemDetails"/> instance conforming to RFC 7807 specifications.
        /// </returns>
        /// <remarks>
        /// Exceptions that lack specific type registrations in the mapper fallback automatically to a generic 500 Internal Server Error problem details structure to avoid leaking sensitive stack traces to API consumers.
        /// </remarks>
        ProblemDetails Map(Exception exception);
    }
}
