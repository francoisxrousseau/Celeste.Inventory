namespace Celeste.Inventory.Application.Features.Queries;

using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

/// <summary>
///	Represents a request to count products.
/// </summary>
/// <param name="SearchText">
///	The optional free-text search.
/// </param>
/// <param name="IncludeDeleted">
///	Indicates whether deleted products may be counted.
/// </param>
public sealed record CountProductsQuery(string? SearchText, bool IncludeDeleted) : IRequest<ProductCountResponse>;
