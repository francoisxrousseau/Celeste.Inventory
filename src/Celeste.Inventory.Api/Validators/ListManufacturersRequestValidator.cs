using Celeste.Inventory.Api.Models.Manufacturers;
using FluentValidation;

namespace Celeste.Inventory.Api.Validators;

/// <summary>
///	Validates manufacturer list requests received by the API.
/// </summary>
public sealed class ListManufacturersRequestValidator : AbstractValidator<ListManufacturersRequest>
{
    /// <summary>
    ///	Initializes a new instance of the <see cref="ListManufacturersRequestValidator"/> class.
    /// </summary>
    public ListManufacturersRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
