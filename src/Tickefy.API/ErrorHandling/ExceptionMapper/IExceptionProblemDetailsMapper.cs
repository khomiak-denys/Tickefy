using Microsoft.AspNetCore.Mvc;

namespace Tickefy.API.ErrorHandling.ExceptionMapper
{
    public interface IExceptionProblemDetailsMapper
    {
        ProblemDetails Map(Exception exception);
    }
}
