using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

namespace Celeste.Inventory.Application.Features.Queries;

/// <summary>
///	Represents a request to fetch a page of manufacturers.
/// </summary>
/// <param name="PageNumber">
///	The 1-based page number.
/// </param>
/// <param name="PageSize">
///	The page size.
/// </param>
/// <param name="SearchText">
///	The optional free-text search.
/// </param>
/// <param name="IncludeDeleted">
///	Indicates whether deleted manufacturers may be returned.
/// </param>
public sealed record ListManufacturersQuery(
    int PageNumber,
    int PageSize,
    string? SearchText,
    bool IncludeDeleted) : IRequest<PagedManufacturersResponse>;
