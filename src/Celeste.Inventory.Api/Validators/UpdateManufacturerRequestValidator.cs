namespace Celeste.Inventory.Api.Validators;

using Celeste.Inventory.Api.Models.Manufacturers;
using FluentValidation;

/// <summary>
///	Validates manufacturer update requests received by the API.
/// </summary>
public sealed class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturerRequest>
{
    /// <summary>
    ///	Initializes a new instance of the <see cref="UpdateManufacturerRequestValidator"/> class.
    /// </summary>
    public UpdateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.ContactEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
    }
}
