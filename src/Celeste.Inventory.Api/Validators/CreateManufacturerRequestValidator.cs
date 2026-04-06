using Celeste.Inventory.Api.Models.Manufacturers;
using FluentValidation;

namespace Celeste.Inventory.Api.Validators;

/// <summary>
///     Validates manufacturer creation requests received by the API.
/// </summary>
public sealed class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturerRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateManufacturerRequestValidator"/> class.
    /// </summary>
    public CreateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.ContactEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
    }
}
