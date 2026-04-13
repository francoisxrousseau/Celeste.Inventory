namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///     Handles variant by-id queries.
/// </summary>
public sealed class GetVariantByIdHandler(IProductRepository repository)
    : IRequestHandler<GetVariantByIdQuery, VariantResponse>
{
    /// <summary>
    ///     Handles the by-id variant query.
    /// </summary>
    /// <param name="request">
    ///     The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///     The matching variant response.
    /// </returns>
    public async Task<VariantResponse> HandleAsync(
        GetVariantByIdQuery request,
        CancellationToken cancellationToken)
    {
        var variant = await repository.GetVariantByIdAsync(request.ProductId, request.VariantId, request.AllowDeleted, cancellationToken);
        if (variant is null)
            throw new VariantNotFoundException("Variant was not found.");

        return variant.ToResponse();
    }
}
