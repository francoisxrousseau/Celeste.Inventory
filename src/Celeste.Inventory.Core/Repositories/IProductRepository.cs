namespace Celeste.Inventory.Core.Repositories;

using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Core.Domain;

/// <summary>
///     Defines data access operations for product aggregates.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    ///     Creates a product.
    /// </summary>
    /// <param name="product">
    ///     The product to persist.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the product has been created.
    /// </returns>
    Task CreateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates a product.
    /// </summary>
    /// <param name="product">
    ///     The product to persist.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the product has been updated.
    /// </returns>
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Atomically updates a product and returns the updated entity.
    /// </summary>
    /// <param name="id">
    ///     The product identifier.
    /// </param>
    /// <param name="manufacturerId">
    ///     The manufacturer identifier.
    /// </param>
    /// <param name="name">
    ///     The updated product name.
    /// </param>
    /// <param name="description">
    ///     The updated optional description.
    /// </param>
    /// <param name="status">
    ///     The updated product status.
    /// </param>
    /// <param name="tags">
    ///     The updated optional tags.
    /// </param>
    /// <param name="updatedBy">
    ///     The user responsible for the update.
    /// </param>
    /// <param name="updatedAt">
    ///     The UTC update timestamp.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     The updated product, or <see langword="null"/> when no active product matches.
    /// </returns>
    Task<Product?> UpdateAsync(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        ProductStatus status,
        IReadOnlyList<string>? tags,
        string? updatedBy,
        DateTime updatedAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Atomically soft deletes a product and returns the deleted entity.
    /// </summary>
    /// <param name="id">
    ///     The product identifier.
    /// </param>
    /// <param name="deletedBy">
    ///     The user responsible for the delete.
    /// </param>
    /// <param name="deletedAt">
    ///     The UTC delete timestamp.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     The deleted product, or <see langword="null"/> when no active product matches.
    /// </returns>
    Task<Product?> DeleteAsync(
        Guid id,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a product by id.
    /// </summary>
    /// <param name="id">
    ///     The product identifier.
    /// </param>
    /// <param name="includeDeleted">
    ///     Indicates whether deleted products may be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     The matching product, or <see langword="null"/> when no match exists.
    /// </returns>
    Task<Product?> GetByIdAsync(
        Guid id,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Searches products using a name substring filter.
    /// </summary>
    /// <param name="searchText">
    ///     The search text, or <see langword="null"/> for all items.
    /// </param>
    /// <param name="pageNumber">
    ///     The 1-based page number.
    /// </param>
    /// <param name="pageSize">
    ///     The page size.
    /// </param>
    /// <param name="includeDeleted">
    ///     Indicates whether deleted products may be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A read-only collection of matching products.
    /// </returns>
    Task<IReadOnlyList<Product>> SearchAsync(
        string? searchText,
        int pageNumber,
        int pageSize,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Counts products matching a name substring filter.
    /// </summary>
    /// <param name="searchText">
    ///     The search text, or <see langword="null"/> for all items.
    /// </param>
    /// <param name="includeDeleted">
    ///     Indicates whether deleted products may be counted.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     The number of matching products.
    /// </returns>
    Task<long> CountAsync(
        string? searchText,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);
}
