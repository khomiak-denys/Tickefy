using Microsoft.AspNetCore.Mvc;
using Tickefy.Domain.Common.Errors;

namespace Tickefy.API.ErrorHandling;

public static class ErrorExtensions
{
    public static IActionResult ToActionResult(this ControllerBase c, Error error)
    {
        var pd = error.ToProblemDetails();
        return c.Problem(pd.Detail, statusCode: pd.Status, title: pd.Title, type: pd.Type);
    }

    private static ProblemDetails ToProblemDetails(this Error error)
    {
        return error switch
        {
            NotFoundError => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = error.Message,
                Type = "https://httpstatuses.io/404"
            },
            ForbiddenError => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = error.Message,
                Type = "https://httpstatuses.io/403"
            },
            AlreadyExistsError => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Already exists",
                Detail = error.Message,
                Type = "https://httpstatuses.io/409"
            },
            InvalidArgumentError => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = error.Message,
                Type = "https://httpstatuses.io/400"
            },
            _ => throw new NotImplementedException()
        };

    }
}