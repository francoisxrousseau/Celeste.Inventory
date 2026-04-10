namespace Celeste.Inventory.Api.ErrorHandling;

using Celeste.Inventory.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Converts known application and domain exceptions into ProblemDetails responses.
/// </summary>
public sealed class ApiExceptionHandler : IExceptionHandler
{
    /// <summary>
    ///     Attempts to translate an exception into an HTTP response.
    /// </summary>
    /// <param name="httpContext">
    ///     The active HTTP context.
    /// </param>
    /// <param name="exception">
    ///     The exception raised while processing the request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> when the exception was handled.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ManufacturerNotFoundException notFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = notFoundException.Message,
            },
            ProductNotFoundException notFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = notFoundException.Message,
            },
            DuplicateManufacturerNameException duplicateException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = duplicateException.Message,
            },
            ValidationException validationException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = validationException.Message,
            },
            ArgumentException argumentException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = argumentException.Message,
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred.",
            },
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
