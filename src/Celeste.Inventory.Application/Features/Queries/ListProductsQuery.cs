namespace Celeste.Inventory.Application.Features.Queries;

using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

/// <summary>
///	Represents a request to fetch a page of products.
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
///	Indicates whether deleted products may be returned.
/// </param>
public sealed record ListProductsQuery(
    int PageNumber,
    int PageSize,
    string? SearchText,
    bool IncludeDeleted) : IRequest<PagedProductsResponse>;
