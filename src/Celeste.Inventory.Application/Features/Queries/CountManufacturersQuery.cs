using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

namespace Celeste.Inventory.Application.Features.Queries;

/// <summary>
///	Represents a request to count manufacturers.
/// </summary>
/// <param name="SearchText">
///	The optional free-text search.
/// </param>
/// <param name="IncludeDeleted">
///	Indicates whether deleted manufacturers may be counted.
/// </param>
public sealed record CountManufacturersQuery(
    string? SearchText,
    bool IncludeDeleted) : IRequest<ManufacturerCountResponse>;
