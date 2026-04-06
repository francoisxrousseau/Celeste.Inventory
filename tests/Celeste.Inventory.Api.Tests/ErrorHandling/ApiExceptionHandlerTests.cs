using System.Text.Json;
using Celeste.Inventory.Api.ErrorHandling;
using Celeste.Inventory.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Celeste.Inventory.Api.Tests.ErrorHandling;

/// <summary>
///     Tests observable API exception handling behavior.
/// </summary>
public sealed class ApiExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_WithDuplicateManufacturerNameException_ReturnsConflictProblemDetails()
    {
        var context = CreateContext();
        var handler = new ApiExceptionHandler();

        var handled = await handler.TryHandleAsync(
            context,
            new DuplicateManufacturerNameException("A manufacturer with the same name already exists."),
            CancellationToken.None);

        var problem = await ReadProblemDetailsAsync(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status409Conflict, problem.Status);
        Assert.Equal("Conflict", problem.Title);
        Assert.Equal("A manufacturer with the same name already exists.", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WithManufacturerNotFoundException_ReturnsNotFoundProblemDetails()
    {
        var context = CreateContext();
        var handler = new ApiExceptionHandler();

        var handled = await handler.TryHandleAsync(
            context,
            new ManufacturerNotFoundException("Manufacturer was not found."),
            CancellationToken.None);

        var problem = await ReadProblemDetailsAsync(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
        Assert.Equal("Not Found", problem.Title);
        Assert.Equal("Manufacturer was not found.", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WithValidationException_ReturnsBadRequestProblemDetails()
    {
        var context = CreateContext();
        var handler = new ApiExceptionHandler();

        var handled = await handler.TryHandleAsync(
            context,
            new ValidationException("Manufacturer name is required."),
            CancellationToken.None);

        var problem = await ReadProblemDetailsAsync(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("Bad Request", problem.Title);
        Assert.Equal("Manufacturer name is required.", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WithUnexpectedException_ReturnsInternalServerErrorProblemDetails()
    {
        var context = CreateContext();
        var handler = new ApiExceptionHandler();

        var handled = await handler.TryHandleAsync(
            context,
            new InvalidOperationException("Unexpected failure."),
            CancellationToken.None);

        var problem = await ReadProblemDetailsAsync(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
        Assert.Equal("Internal Server Error", problem.Title);
        Assert.Equal("An unexpected error occurred.", problem.Detail);
    }

    private static DefaultHttpContext CreateContext()
    {
        return new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream(),
            },
        };
    }

    private static async Task<ProblemDetails> ReadProblemDetailsAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;

        return (await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            cancellationToken: CancellationToken.None))!;
    }
}
