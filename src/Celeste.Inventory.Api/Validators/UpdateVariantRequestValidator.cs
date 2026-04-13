namespace Celeste.Inventory.Api.Validators;

using Celeste.Inventory.Api.Models.Products;
using FluentValidation;

/// <summary>
///     Validates variant update requests received by the API.
/// </summary>
public sealed class UpdateVariantRequestValidator : AbstractValidator<UpdateVariantRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UpdateVariantRequestValidator"/> class.
    /// </summary>
    public UpdateVariantRequestValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum()
            .NotEqual((Common.Enums.ProductStatus)0);

        RuleForEach(x => x.Attributes)
            .ChildRules(attribute =>
            {
                attribute.RuleFor(x => x!.Name).NotEmpty();
                attribute.RuleFor(x => x!.Value).NotEmpty();
            });

        RuleFor(x => x.DiscountInformations)
            .ChildRules(discount =>
            {
                discount.RuleFor(x => x!.DiscountPercentage).GreaterThanOrEqualTo(0);
            })
            .When(x => x.DiscountInformations is not null);
    }
}
