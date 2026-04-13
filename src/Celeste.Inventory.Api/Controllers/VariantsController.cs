namespace Celeste.Inventory.Api.Controllers;

using Celeste.Inventory.Api.Authorization;
using Celeste.Inventory.Api.Models.Products;
using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Core.Domain;
using Emit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Exposes variant management endpoints under a product resource.
/// </summary>
[ApiController]
[Route("products/{productId:guid}/variants")]
public sealed class VariantsController(IMediator mediator) : ControllerBase
{
    private const string GetVariantByIdRouteName = "GetVariantById";

    /// <summary>
    ///     Creates a variant for the specified product.
    /// </summary>
    /// <param name="productId">
    ///     The product identifier.
    /// </param>
    /// <param name="request">
    ///     The variant payload from the request body.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The created variant response.
    /// </returns>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.ProductWrite)]
    [ProducesResponseType<VariantResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VariantResponse>> CreateAsync(
        Guid productId,
        [FromBody] CreateVariantRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new CreateVariantCommand(
                productId,
                request.Sku,
                request.Price,
                request.DiscountInformations?.ToDomain(),
                request.Status,
                request.Attributes?.Select(attribute => attribute.ToDomain()).ToList(),
                DateTime.UtcNow),
            cancellationToken);
        var apiResponse = response.ToApiModel();

        return CreatedAtRoute(GetVariantByIdRouteName, new { productId, variantId = apiResponse.Id }, apiResponse);
    }

    /// <summary>
    ///     Gets a variant for a product by identifier.
    /// </summary>
    /// <param name="productId">
    ///     The product identifier.
    /// </param>
    /// <param name="variantId">
    ///     The variant identifier.
    /// </param>
    /// <param name="allowDeleted">
    ///     Indicates whether deleted variants may be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The variant response.
    /// </returns>
    [HttpGet("{variantId:guid}", Name = GetVariantByIdRouteName)]
    [Authorize(Policy = AuthorizationPolicies.ProductRead)]
    [ProducesResponseType<VariantResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VariantResponse>> GetByIdAsync(
        Guid productId,
        Guid variantId,
        [FromQuery] bool allowDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new GetVariantByIdQuery(productId, variantId, allowDeleted),
            cancellationToken);

        return Ok(response.ToApiModel());
    }

    /// <summary>
    ///     Lists variants for a product.
    /// </summary>
    /// <param name="productId">
    ///     The product identifier.
    /// </param>
    /// <param name="includeDeleted">
    ///     Indicates whether deleted variants may be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The variant response collection.
    /// </returns>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ProductRead)]
    [ProducesResponseType<IReadOnlyList<VariantResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListAsync(
        Guid productId,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new ListVariantsQuery(productId, includeDeleted),
            cancellationToken);

        return Ok(response.Select(item => item.ToApiModel()).ToList());
    }

    /// <summary>
    ///     Updates a variant for the specified product.
    /// </summary>
    /// <param name="productId">
    ///     The product identifier.
    /// </param>
    /// <param name="variantId">
    ///     The variant identifier.
    /// </param>
    /// <param name="request">
    ///     The updated variant payload.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     The updated variant response.
    /// </returns>
    [HttpPut("{variantId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ProductWrite)]
    [ProducesResponseType<VariantResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VariantResponse>> UpdateAsync(
        Guid productId,
        Guid variantId,
        [FromBody] UpdateVariantRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.SendAsync(
            new UpdateVariantCommand(
                productId,
                variantId,
                request.Sku,
                request.Price,
                request.DiscountInformations?.ToDomain(),
                request.Status,
                request.Attributes?.Select(attribute => attribute.ToDomain()).ToList(),
                DateTime.UtcNow),
            cancellationToken);

        return Ok(response.ToApiModel());
    }

    /// <summary>
    ///     Soft deletes a variant.
    /// </summary>
    /// <param name="productId">
    ///     The product identifier.
    /// </param>
    /// <param name="variantId">
    ///     The variant identifier.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the request.
    /// </param>
    /// <returns>
    ///     A no-content response when the delete succeeds.
    /// </returns>
    [HttpDelete("{variantId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ProductWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken = default)
    {
        await mediator.SendAsync(
            new DeleteVariantCommand(
                productId,
                variantId,
                DateTime.UtcNow),
            cancellationToken);

        return NoContent();
    }
}

internal static class VariantRequestMappingExtensions
{
    /// <summary>
    ///     Maps a variant request discount to the core domain object.
    /// </summary>
    /// <param name="request">
    ///     The request model to map.
    /// </param>
    /// <returns>
    ///     The mapped domain object.
    /// </returns>
    public static DiscountInformations ToDomain(this DiscountInformationsRequest request)
    {
        return new DiscountInformations
        {
            DiscountPercentage = request.DiscountPercentage,
            DiscountStartAtUtc = request.DiscountStartAtUtc,
            DiscountEndAtUtc = request.DiscountEndAtUtc,
        };
    }

    /// <summary>
    ///     Maps a variant request attribute to the core domain object.
    /// </summary>
    /// <param name="request">
    ///     The request model to map.
    /// </param>
    /// <returns>
    ///     The mapped domain object.
    /// </returns>
    public static VariantAttribute ToDomain(this VariantAttributeRequest request)
    {
        return new VariantAttribute
        {
            Name = request.Name,
            Value = request.Value,
        };
    }
}
