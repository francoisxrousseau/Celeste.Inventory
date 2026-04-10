namespace Celeste.Inventory.Api.Validators;

using Celeste.Inventory.Api.Models.Products;
using FluentValidation;

/// <summary>
///	Validates product list requests received by the API.
/// </summary>
public sealed class ListProductsRequestValidator : AbstractValidator<ListProductsRequest>
{
    /// <summary>
    ///	Initializes a new instance of the <see cref="ListProductsRequestValidator"/> class.
    /// </summary>
    public ListProductsRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
