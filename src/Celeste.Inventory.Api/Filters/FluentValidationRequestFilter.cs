using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Celeste.Inventory.Api.Filters;

/// <summary>
///     Executes registered FluentValidation validators for bound action arguments.
/// </summary>
public sealed class FluentValidationRequestFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    /// <summary>
    ///     Validates action arguments before the controller action executes.
    /// </summary>
    /// <param name="context">
    ///     The action execution context.
    /// </param>
    /// <param name="next">
    ///     The continuation delegate for the remaining pipeline.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = CreateValidationContext(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
            if (!result.IsValid)
            {
                context.Result = new BadRequestObjectResult(CreateValidationProblemDetails(result));
                return;
            }
        }

        await next();
    }

    private static IValidationContext CreateValidationContext(object argument)
    {
        return (IValidationContext)Activator.CreateInstance(
            typeof(ValidationContext<>).MakeGenericType(argument.GetType()),
            argument)!;
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(ValidationResult result)
    {
        var errors = result.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
        };
    }
}
