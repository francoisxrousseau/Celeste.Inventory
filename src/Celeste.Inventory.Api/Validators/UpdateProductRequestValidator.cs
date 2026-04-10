namespace Celeste.Inventory.Api.Validators;

using Celeste.Inventory.Api.Models.Products;
using FluentValidation;

/// <summary>
///	Validates product update requests received by the API.
/// </summary>
public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    /// <summary>
    ///	Initializes a new instance of the <see cref="UpdateProductRequestValidator"/> class.
    /// </summary>
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.ManufacturerId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleForEach(x => x.Tags)
            .NotEmpty();
    }
}
