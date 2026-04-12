namespace Celeste.Inventory.Application.Tests.Features.Handlers;

using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Application.Features.Handlers;
using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Identity;
using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Core.Repositories;
using Emit.Abstractions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
///     Tests observable product application handler behavior.
/// </summary>
public sealed class ProductHandlersTests
{
    [Fact]
    public async Task CreateHandler_WithValidPayload_CreatesProductAndPublishesEvent()
    {
        var repository = new FakeProductRepository();
        var publisher = new FakeProductEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUserAccessor { UserId = "alice" };
        var handler = new CreateProductHandler(
            repository,
            currentUser,
            publisher,
            unitOfWork,
            NullLogger<CreateProductHandler>.Instance);

        var response = await handler.HandleAsync(
            new CreateProductCommand(
                Guid.NewGuid(),
                "  Celeste Tee  ",
                "  Soft cotton shirt  ",
                ProductStatus.Active,
                ProductCategory.Apparel,
                ["apparel", "cotton"],
                Utc(2026, 4, 9, 12, 0)),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Celeste Tee", response.Name);
        Assert.Equal("Soft cotton shirt", response.Description);
        Assert.Equal(ProductStatus.Active, response.Status);
        Assert.Equal(ProductCategory.Apparel, response.Category);
        Assert.Equal(2, response.Tags?.Count);
        Assert.Single(repository.Items);
        Assert.Equal("alice", repository.Items[0].CreatedBy);
        Assert.Equal(1, publisher.CreatedEvents.Count);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(1, unitOfWork.CommitCalls);
    }

    [Fact]
    public async Task UpdateHandler_WithExistingProduct_UpdatesFields()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            ManufacturerId = Guid.NewGuid(),
            Name = "Old Product",
            Description = "old",
            Status = ProductStatus.Inactive,
            Category = ProductCategory.Accessories,
            Tags = ["old"],
            CreatedBy = "alice",
            CreatedAt = Utc(2026, 4, 1, 8, 0),
        };

        var repository = new FakeProductRepository();
        repository.Items.Add(product);
        var publisher = new FakeProductEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUserAccessor { UserId = "bob" };
        var handler = new UpdateProductHandler(
            repository,
            currentUser,
            publisher,
            unitOfWork,
            NullLogger<UpdateProductHandler>.Instance);

        var response = await handler.HandleAsync(
            new UpdateProductCommand(
                product.Id,
                product.ManufacturerId,
                "  New Product  ",
                "  desc  ",
                ProductStatus.Active,
                ProductCategory.Apparel,
                ["new", "featured"],
                Utc(2026, 4, 9, 13, 0)),
            CancellationToken.None);

        Assert.Equal(product.Id, response.Id);
        Assert.Equal("New Product", response.Name);
        Assert.Equal("desc", response.Description);
        Assert.Equal(ProductStatus.Active, response.Status);
        Assert.Equal(ProductCategory.Apparel, response.Category);
        Assert.Equal("bob", product.LastUpdatedBy);
        Assert.Equal(Utc(2026, 4, 9, 13, 0), product.LastUpdatedAt);
        Assert.Equal(1, publisher.UpdatedEvents.Count);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(1, unitOfWork.CommitCalls);
    }

    [Fact]
    public async Task UpdateHandler_WhenMissing_ThrowsProductNotFoundException()
    {
        var repository = new FakeProductRepository();
        var publisher = new FakeProductEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUserAccessor { UserId = "bob" };
        var handler = new UpdateProductHandler(
            repository,
            currentUser,
            publisher,
            unitOfWork,
            NullLogger<UpdateProductHandler>.Instance);

        var action = () => handler.HandleAsync(
            new UpdateProductCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Product",
                null,
                ProductStatus.Active,
                ProductCategory.Apparel,
                null,
                Utc(2026, 4, 9, 13, 0)),
            CancellationToken.None);

        await Assert.ThrowsAsync<ProductNotFoundException>(action);
        Assert.Equal(0, publisher.TotalPublished);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(0, unitOfWork.CommitCalls);
    }

    [Fact]
    public async Task GetByIdHandler_WithDeletedAllowed_ReturnsProduct()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            ManufacturerId = Guid.NewGuid(),
            Name = "Celeste Tee",
            Status = ProductStatus.Active,
            Category = ProductCategory.Apparel,
            CreatedAt = Utc(2026, 4, 1, 8, 0),
            DeletedAt = Utc(2026, 4, 2, 8, 0),
        };

        var repository = new FakeProductRepository();
        repository.Items.Add(product);
        var handler = new GetProductByIdHandler(repository);

        var response = await handler.HandleAsync(new GetProductByIdQuery(product.Id, true), CancellationToken.None);

        Assert.Equal(product.Id, response.Id);
        Assert.Equal("Celeste Tee", response.Name);
    }

    [Fact]
    public async Task ListHandler_ReturnsPagedResponseAndTrimsSearchText()
    {
        var repository = new FakeProductRepository();
        repository.Items.AddRange(
        [
            new Product { Id = Guid.NewGuid(), ManufacturerId = Guid.NewGuid(), Name = "Celeste Tee", Status = ProductStatus.Active, Category = ProductCategory.Apparel, CreatedAt = Utc(2026, 4, 1, 8, 0) },
            new Product { Id = Guid.NewGuid(), ManufacturerId = Guid.NewGuid(), Name = "Celestial Cap", Status = ProductStatus.Active, Category = ProductCategory.Apparel, CreatedAt = Utc(2026, 4, 1, 8, 0) },
            new Product { Id = Guid.NewGuid(), ManufacturerId = Guid.NewGuid(), Name = "Northern Boots", Status = ProductStatus.Active, Category = ProductCategory.Footwear, CreatedAt = Utc(2026, 4, 1, 8, 0) },
        ]);

        var handler = new ListProductsHandler(repository);

        var response = await handler.HandleAsync(
            new ListProductsQuery(1, 10, "  celes  ", false),
            CancellationToken.None);

        Assert.Equal(2, response.TotalCount);
        Assert.Equal(2, response.Items.Count);
        Assert.Equal("CELES", repository.LastSearchText);
    }

    [Fact]
    public async Task CountHandler_WithWhitespaceSearch_PassesNullSearchText()
    {
        var repository = new FakeProductRepository();
        repository.Items.Add(new Product { Id = Guid.NewGuid(), ManufacturerId = Guid.NewGuid(), Name = "Celeste Tee", Status = ProductStatus.Active, Category = ProductCategory.Apparel, CreatedAt = Utc(2026, 4, 1, 8, 0) });
        repository.Items.Add(new Product { Id = Guid.NewGuid(), ManufacturerId = Guid.NewGuid(), Name = "Northern Boots", Status = ProductStatus.Active, Category = ProductCategory.Footwear, CreatedAt = Utc(2026, 4, 1, 8, 0) });

        var handler = new CountProductsHandler(repository);

        var response = await handler.HandleAsync(
            new CountProductsQuery("   ", false),
            CancellationToken.None);

        Assert.Equal(2, response.TotalCount);
        Assert.Null(repository.LastCountSearchText);
    }

    [Fact]
    public async Task DeleteHandler_WhenProductExists_SetsDeleteAuditFields()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            ManufacturerId = Guid.NewGuid(),
            Name = "Celeste Tee",
            Status = ProductStatus.Active,
            Category = ProductCategory.Apparel,
            CreatedAt = Utc(2026, 4, 1, 8, 0),
        };

        var repository = new FakeProductRepository();
        repository.Items.Add(product);
        var publisher = new FakeProductEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUserAccessor { UserId = "charlie" };
        var handler = new DeleteProductHandler(
            repository,
            currentUser,
            publisher,
            unitOfWork,
            NullLogger<DeleteProductHandler>.Instance);

        await handler.HandleAsync(
            new DeleteProductCommand(product.Id, Utc(2026, 4, 9, 14, 0)),
            CancellationToken.None);

        Assert.Equal("charlie", product.DeletedBy);
        Assert.Equal(Utc(2026, 4, 9, 14, 0), product.DeletedAt);
        Assert.True(product.IsDeleted);
        Assert.Equal(1, publisher.DeletedEvents.Count);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(1, unitOfWork.CommitCalls);
    }

    [Fact]
    public async Task DeleteHandler_WhenProductMissing_ThrowsProductNotFoundException()
    {
        var repository = new FakeProductRepository();
        var publisher = new FakeProductEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUserAccessor { UserId = "charlie" };
        var handler = new DeleteProductHandler(
            repository,
            currentUser,
            publisher,
            unitOfWork,
            NullLogger<DeleteProductHandler>.Instance);

        var action = () => handler.HandleAsync(
            new DeleteProductCommand(Guid.NewGuid(), Utc(2026, 4, 9, 14, 0)),
            CancellationToken.None);

        await Assert.ThrowsAsync<ProductNotFoundException>(action);
        Assert.Equal(0, publisher.TotalPublished);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(0, unitOfWork.CommitCalls);
    }

    private static DateTime Utc(int year, int month, int day, int hour, int minute)
    {
        return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Items { get; } = [];

        public string? LastSearchText { get; private set; }

        public string? LastCountSearchText { get; private set; }

        public Task<long> CountAsync(string? searchText, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            LastCountSearchText = searchText;
            var count = ApplyFilter(searchText, includeDeleted).LongCount();
            return Task.FromResult(count);
        }

        public Task CreateAsync(Product product, CancellationToken cancellationToken = default)
        {
            Items.Add(product);
            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            var item = Items.SingleOrDefault(x => x.Id == id && (includeDeleted || !x.IsDeleted));
            return Task.FromResult(item);
        }

        public Task<Product?> DeleteAsync(Guid id, string? deletedBy, DateTime deletedAt, CancellationToken cancellationToken = default)
        {
            var item = Items.SingleOrDefault(x => x.Id == id && !x.IsDeleted);
            if (item is null)
            {
                return Task.FromResult<Product?>(null);
            }

            item.DeletedBy = deletedBy;
            item.DeletedAt = deletedAt;
            return Task.FromResult<Product?>(item);
        }

        public Task<IReadOnlyList<Product>> SearchAsync(
            string? searchText,
            int pageNumber,
            int pageSize,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            LastSearchText = searchText;
            var items = ApplyFilter(searchText, includeDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult<IReadOnlyList<Product>>(items);
        }

        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<Product?> UpdateAsync(
            Guid id,
            Guid manufacturerId,
            string name,
            string? description,
            ProductStatus status,
            ProductCategory category,
            IReadOnlyList<string>? tags,
            string? updatedBy,
            DateTime updatedAt,
            CancellationToken cancellationToken = default)
        {
            var item = Items.SingleOrDefault(x => x.Id == id && !x.IsDeleted);
            if (item is null)
            {
                return Task.FromResult<Product?>(null);
            }

            item.ManufacturerId = manufacturerId;
            item.Name = name;
            item.Description = description;
            item.Status = status;
            item.Category = category;
            item.Tags = tags;
            item.LastUpdatedBy = updatedBy;
            item.LastUpdatedAt = updatedAt;

            return Task.FromResult<Product?>(item);
        }

        private IEnumerable<Product> ApplyFilter(string? searchText, bool includeDeleted)
        {
            return Items.Where(x => includeDeleted || !x.IsDeleted)
                .Where(x => searchText is null || Normalize(x.Name).Contains(Normalize(searchText), StringComparison.Ordinal));
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToUpperInvariant();
        }
    }

    private sealed class FakeProductEventPublisher : IProductEventPublisher
    {
        public List<Product> CreatedEvents { get; } = [];

        public List<Product> UpdatedEvents { get; } = [];

        public List<(Product Product, string? DeletedBy, DateTime DeletedAt)> DeletedEvents { get; } = [];

        public int TotalPublished => CreatedEvents.Count + UpdatedEvents.Count + DeletedEvents.Count;

        public Task PublishCreatedAsync(Product product, CancellationToken cancellationToken = default)
        {
            CreatedEvents.Add(product);
            return Task.CompletedTask;
        }

        public Task PublishUpdatedAsync(Product product, CancellationToken cancellationToken = default)
        {
            UpdatedEvents.Add(product);
            return Task.CompletedTask;
        }

        public Task PublishDeletedAsync(
            Product product,
            string? deletedBy,
            DateTime deletedAt,
            CancellationToken cancellationToken = default)
        {
            DeletedEvents.Add((product, deletedBy, deletedAt));
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int BeginCalls { get; private set; }

        public int CommitCalls { get; private set; }

        public ValueTask<IUnitOfWorkTransaction> BeginAsync(CancellationToken cancellationToken = default)
        {
            BeginCalls++;
            return ValueTask.FromResult<IUnitOfWorkTransaction>(new FakeUnitOfWorkTransaction(this));
        }

        private sealed class FakeUnitOfWorkTransaction(FakeUnitOfWork owner) : IUnitOfWorkTransaction
        {
            public bool IsCommitted { get; private set; }

            public bool IsRolledBack { get; private set; }

            public Task CommitAsync(CancellationToken cancellationToken = default)
            {
                IsCommitted = true;
                owner.CommitCalls++;
                return Task.CompletedTask;
            }

            public Task RollbackAsync(CancellationToken cancellationToken = default)
            {
                IsRolledBack = true;
                return Task.CompletedTask;
            }

            public ValueTask DisposeAsync()
            {
                if (!IsCommitted)
                    IsRolledBack = true;

                return ValueTask.CompletedTask;
            }
        }
    }

    private sealed class FakeCurrentUserAccessor : ICurrentUserAccessor
    {
        public string? UserId { get; init; }

        public string? Name { get; init; }
    }
}
