using Microsoft.AspNetCore.Mvc;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;

namespace Tickefy.API.ErrorHandling;

public static class ResultExtensions
{
    public static IActionResult Match(this Result resultor, IActionResult result, Func<Error, IActionResult> onFailure)
    {
        return resultor.IsSuccess ? result : onFailure(resultor.Error);
    }
}
