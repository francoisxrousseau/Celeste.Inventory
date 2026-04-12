namespace Celeste.Inventory.Api.Controllers;

using Celeste.Inventory.Api.Authorization;
using Celeste.Inventory.Api.Models.Products;
using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Application.Features.Queries;
using Emit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Exposes product management endpoints.
/// </summary>
[ApiController]
[Route("products")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    private const string GetProductByIdRouteName = "GetProductById";

    /// <summary>
    ///     Creates a product by delegating to the application layer.
    /// </summary>
    /// <param name="request">
    ///     The product payload from the request body.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The created product response.
    /// </returns>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.ProductWrite)]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> CreateAsync(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new CreateProductCommand(
                request.ManufacturerId,
                request.Name,
                request.Description,
                request.Status,
                request.Category,
                request.Tags,
                DateTime.UtcNow),
            cancellationToken);
        var apiResponse = response.ToApiModel();

        return CreatedAtRoute(GetProductByIdRouteName, new { id = apiResponse.Id }, apiResponse);
    }

    /// <summary>
    ///     Gets a product by identifier.
    /// </summary>
    /// <param name="id">
    ///     The product identifier.
    /// </param>
    /// <param name="allowDeleted">
    ///     Indicates whether deleted products may be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The product response.
    /// </returns>
    [HttpGet("{id:guid}", Name = GetProductByIdRouteName)]
    [Authorize(Policy = AuthorizationPolicies.ProductRead)]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> GetByIdAsync(
        Guid id,
        [FromQuery] bool allowDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new GetProductByIdQuery(id, allowDeleted),
            cancellationToken);

        return Ok(response.ToApiModel());
    }

    /// <summary>
    ///     Lists products or returns a count-only response.
    /// </summary>
    /// <param name="request">
    ///     The list query parameters.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     Either a paged product response or a count-only response.
    /// </returns>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ProductRead)]
    [ProducesResponseType<PagedProductsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProductCountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] ListProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CountOnly)
        {
            var count = await mediator.SendAsync(
                new CountProductsQuery(request.SearchText, request.IncludeDeleted),
                cancellationToken);

            return Ok(count.ToApiModel());
        }

        var response = await mediator.SendAsync(
            new ListProductsQuery(
                request.PageNumber,
                request.PageSize,
                request.SearchText,
                request.IncludeDeleted),
            cancellationToken);

        return Ok(response.ToApiModel());
    }

    /// <summary>
    ///     Updates a product by delegating to the application layer.
    /// </summary>
    /// <param name="id">
    ///     The product identifier.
    /// </param>
    /// <param name="request">
    ///     The updated product payload.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The updated product response.
    /// </returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ProductWrite)]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> UpdateAsync(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new UpdateProductCommand(
                id,
                request.ManufacturerId,
                request.Name,
                request.Description,
                request.Status,
                request.Category,
                request.Tags,
                DateTime.UtcNow),
            cancellationToken);

        return Ok(response.ToApiModel());
    }

    /// <summary>
    ///     Soft deletes a product.
    /// </summary>
    /// <param name="id">
    ///     The product identifier.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     A no-content response when the delete succeeds.
    /// </returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ProductWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await mediator.SendAsync(
            new DeleteProductCommand(
                id,
                DateTime.UtcNow),
            cancellationToken);

        return NoContent();
    }
}
