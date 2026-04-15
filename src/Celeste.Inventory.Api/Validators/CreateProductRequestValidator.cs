namespace Celeste.Inventory.Api.Validators;

using Celeste.Inventory.Api.Models.Products;
using FluentValidation;

/// <summary>
///     Validates product creation requests received by the API.
/// </summary>
public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateProductRequestValidator"/> class.
    /// </summary>
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.ManufacturerId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Category)
            .IsInEnum()
            .NotEqual((Common.Enums.ProductCategory)0);

        RuleForEach(x => x.Tags)
            .NotEmpty();

        When(x => x.Variant is not null, () =>
        {
            RuleFor(x => x.Variant!)
                .SetValidator(new CreateVariantRequestValidator());
        });
    }
}
