using Celeste.Inventory.Api.Controllers;
using Celeste.Inventory.Api.Models.Manufacturers;
using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Common.Responses;
using Emit.Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Celeste.Inventory.Api.Tests.Controllers;

/// <summary>
///     Tests observable manufacturer controller behavior.
/// </summary>
public sealed class ManufacturersControllerTests
{
    [Fact]
    public async Task Create_WithValidRequest_DelegatesToMediatorAndReturnsCreatedResponse()
    {
        var mediator = new FakeMediator();
        var controller = CreateController(mediator);
        var request = new CreateManufacturerRequest
        {
            Name = "Celeste Labs",
            ContactEmail = "contact@celeste.test",
            ContactPhone = "4165551234",
        };

        var result = await controller.CreateAsync(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ManufacturerResponse>(created.Value);
        var command = Assert.IsType<CreateManufacturerCommand>(mediator.LastRequest);

        Assert.Equal("Celeste Labs", command.Name);
        Assert.Equal("contact@celeste.test", command.ContactEmail);
        Assert.Equal("4165551234", command.ContactPhone);
        Assert.Equal(response.Id, created.RouteValues!["id"]);
        Assert.Equal(nameof(ManufacturersController.GetByIdAsync), created.ActionName);
    }

    private static ManufacturersController CreateController(FakeMediator mediator)
    {
        return new ManufacturersController(mediator)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };
    }

    private sealed class FakeMediator : IMediator
    {
        public object? LastRequest { get; private set; }

        public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.CompletedTask;
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;

            return Task.FromResult((TResponse)(object)new ManufacturerResponse(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Celeste Labs",
                "contact@celeste.test",
                "4165551234"));
        }
    }
}
