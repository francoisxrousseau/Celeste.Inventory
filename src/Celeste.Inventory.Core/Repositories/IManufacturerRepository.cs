using Celeste.Inventory.Core.Domain;

namespace Celeste.Inventory.Core.Repositories;

/// <summary>
///     Defines data access operations for manufacturer aggregates.
/// </summary>
public interface IManufacturerRepository
{
    /// <summary>
    ///     Creates a manufacturer.
    /// </summary>
    /// <param name="manufacturer">
    ///     The manufacturer to persist.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the manufacturer has been created.
    /// </returns>
    Task CreateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates a manufacturer.
    /// </summary>
    /// <param name="manufacturer">
    ///     The manufacturer to persist.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the manufacturer has been updated.
    /// </returns>
    Task UpdateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Atomically updates a manufacturer and returns the updated entity.
    /// </summary>
    /// <param name="id">
    ///     The manufacturer identifier.
    /// </param>
    /// <param name="name">
    ///     The updated manufacturer name.
    /// </param>
    /// <param name="contactEmail">
    ///     The updated optional contact email.
    /// </param>
    /// <param name="contactPhone">
    ///     The updated optional contact phone number.
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
    ///     The updated manufacturer, or <see langword="null"/> when no active manufacturer matches.
    /// </returns>
    Task<Manufacturer?> UpdateAsync(
        Guid id,
        string name,
        string? contactEmail,
        string? contactPhone,
        string? updatedBy,
        DateTime updatedAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Atomically soft deletes a manufacturer.
    /// </summary>
    /// <param name="id">
    ///     The manufacturer identifier.
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
    ///     <see langword="true"/> when a manufacturer was deleted; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> DeleteAsync(
        Guid id,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a manufacturer by id.
    /// </summary>
    /// <param name="id">
    ///     The manufacturer identifier.
    /// </param>
    /// <param name="includeDeleted">
    ///     Indicates whether deleted manufacturers may be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     The matching manufacturer, or <see langword="null"/> when no match exists.
    /// </returns>
    Task<Manufacturer?> GetByIdAsync(
        Guid id,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether an active manufacturer already exists with the supplied name.
    /// </summary>
    /// <param name="name">
    ///     The manufacturer name to check.
    /// </param>
    /// <param name="excludedId">
    ///     An optional identifier to exclude from the check.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> when a duplicate exists; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Searches manufacturers using a name substring filter.
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
    ///     Indicates whether deleted manufacturers may be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A read-only collection of matching manufacturers.
    /// </returns>
    Task<IReadOnlyList<Manufacturer>> SearchAsync(
        string? searchText,
        int pageNumber,
        int pageSize,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Counts manufacturers matching a name substring filter.
    /// </summary>
    /// <param name="searchText">
    ///     The search text, or <see langword="null"/> for all items.
    /// </param>
    /// <param name="includeDeleted">
    ///     Indicates whether deleted manufacturers may be counted.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     The number of matching manufacturers.
    /// </returns>
    Task<long> CountAsync(
        string? searchText,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);
}
