using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Core.Presentation.Controllers;

/// <summary>
/// Base controller for every service API (EcoGreen's <c>ApiController</c>, promoted to Core).
/// <see cref="Problem(IReadOnlyList{Error})"/> maps ErrorOr errors to Problem Details:
/// all-validation errors become a 400 with field errors, anything else uses the first error's type.
/// </summary>
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    public const string ErrorCodesKey = "errorCodes";

    protected ActionResult Problem(IReadOnlyList<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    protected ActionResult Problem(List<Error> errors) => Problem((IReadOnlyList<Error>)errors);

    private ObjectResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        // Unexpected/failure details are not leaked to clients; the code is still returned for support/tracing.
        var detail = statusCode == StatusCodes.Status500InternalServerError ? null : error.Description;
        var result = Problem(statusCode: statusCode, title: TitleFor(statusCode), detail: detail);

        if (result.Value is ProblemDetails problemDetails)
        {
            problemDetails.Extensions[ErrorCodesKey] = new[] { error.Code };
        }

        return result;
    }

    private ActionResult ValidationProblem(IReadOnlyList<Error> errors)
    {
        var modelState = new ModelStateDictionary();
        foreach (var error in errors)
        {
            // "Order.Lines.Empty" → field "Order.Lines"; codes without a dot are used as-is.
            var lastDot = error.Code.LastIndexOf('.');
            var field = lastDot > 0 ? error.Code[..lastDot] : error.Code;
            modelState.AddModelError(field, error.Description);
        }

        var result = ValidationProblem(modelState);
        if (result is ObjectResult { Value: ProblemDetails problemDetails })
        {
            problemDetails.Extensions[ErrorCodesKey] = errors.Select(error => error.Code).ToArray();
        }

        return result;
    }

    private static string TitleFor(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Bad request",
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status403Forbidden => "Forbidden",
        StatusCodes.Status404NotFound => "Resource not found",
        StatusCodes.Status409Conflict => "Conflict",
        _ => "An unexpected error occurred",
    };
}
