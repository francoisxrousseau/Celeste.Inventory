using Celeste.Inventory.Api.Models.Manufacturers;
using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Common.Responses;
using Emit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Celeste.Inventory.Api.Controllers;

/// <summary>
///     Exposes manufacturer management endpoints.
/// </summary>
[ApiController]
[Route("manufacturers")]
public sealed class ManufacturersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    ///     Creates a manufacturer by delegating to the application layer.
    /// </summary>
    /// <param name="request">
    ///     The manufacturer payload from the request body.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The created manufacturer response.
    /// </returns>
    [HttpPost]
    [ProducesResponseType<ManufacturerResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ManufacturerResponse>> CreateAsync(
        [FromBody] CreateManufacturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new CreateManufacturerCommand(
                request.Name,
                request.ContactEmail,
                request.ContactPhone,
                User.Identity?.Name,
                DateTime.UtcNow),
            cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    /// <summary>
    ///     Gets a manufacturer by identifier.
    /// </summary>
    /// <param name="id">
    ///     The manufacturer identifier.
    /// </param>
    /// <param name="allowDeleted">
    ///     Indicates whether deleted manufacturers may be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The manufacturer response.
    /// </returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ManufacturerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ManufacturerResponse>> GetByIdAsync(
        Guid id,
        [FromQuery] bool allowDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new GetManufacturerByIdQuery(id, allowDeleted),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    ///     Lists manufacturers or returns a count-only response.
    /// </summary>
    /// <param name="request">
    ///     The list query parameters.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     Either a paged manufacturer response or a count-only response.
    /// </returns>
    [HttpGet]
    [ProducesResponseType<PagedManufacturersResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ManufacturerCountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] ListManufacturersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CountOnly)
        {
            var count = await mediator.SendAsync(
                new CountManufacturersQuery(request.SearchText, request.IncludeDeleted),
                cancellationToken);

            return Ok(count);
        }

        var response = await mediator.SendAsync(
            new ListManufacturersQuery(
                request.PageNumber,
                request.PageSize,
                request.SearchText,
                request.IncludeDeleted),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    ///     Updates a manufacturer by delegating to the application layer.
    /// </summary>
    /// <param name="id">
    ///     The manufacturer identifier.
    /// </param>
    /// <param name="request">
    ///     The updated manufacturer payload.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The updated manufacturer response.
    /// </returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<ManufacturerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ManufacturerResponse>> UpdateAsync(
        Guid id,
        [FromBody] UpdateManufacturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new UpdateManufacturerCommand(
                id,
                request.Name,
                request.ContactEmail,
                request.ContactPhone,
                User.Identity?.Name,
                DateTime.UtcNow),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    ///     Soft deletes a manufacturer.
    /// </summary>
    /// <param name="id">
    ///     The manufacturer identifier.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     A no-content response when the delete succeeds.
    /// </returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await mediator.SendAsync(
            new DeleteManufacturerCommand(
                id,
                User.Identity?.Name,
                DateTime.UtcNow),
            cancellationToken);

        return NoContent();
    }
}
