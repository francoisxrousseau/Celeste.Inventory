namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///	Handles product by-id queries.
/// </summary>
public sealed class GetProductByIdHandler(IProductRepository repository)
    : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    /// <summary>
    ///	Handles the by-id product query.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The matching product response.
    /// </returns>
    public async Task<ProductResponse> HandleAsync(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, request.AllowDeleted, cancellationToken);
        if (product is null)
        {
            throw new ProductNotFoundException("Product was not found.");
        }

        return product.ToResponse();
    }
}
