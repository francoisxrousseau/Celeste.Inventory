using Celeste.Inventory.Api.ErrorHandling;
using Celeste.Inventory.Api.Filters;
using Celeste.Inventory.Api.Installers;
using Celeste.Inventory.Api.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<FluentValidationRequestFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationRequestFilter>();
});
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<CreateManufacturerRequestValidator>();
builder.Services.AddManufacturerBootstrap(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
