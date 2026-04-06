using Celeste.Inventory.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Celeste.Inventory.Api.ErrorHandling;

/// <summary>
///     Converts known application and domain exceptions into RFC 7807 responses.
/// </summary>
public sealed class ApiExceptionHandler : IExceptionHandler
{
    /// <summary>
    ///     Attempts to handle the supplied exception for the current HTTP request.
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
    ///     <see langword="true"/> when the exception was translated into a response.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(exception);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(Exception exception)
    {
        return exception switch
        {
            DuplicateManufacturerNameException duplicateException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = duplicateException.Message,
            },
            ArgumentException argumentException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = argumentException.Message,
            },
            ValidationException validationException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = validationException.Message,
            },
            _ when exception.GetType().Name.EndsWith("NotFoundException", StringComparison.Ordinal) => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = exception.Message,
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred.",
            },
        };
    }
}
