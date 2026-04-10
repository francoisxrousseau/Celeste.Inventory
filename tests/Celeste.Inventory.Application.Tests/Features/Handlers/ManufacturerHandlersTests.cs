namespace Celeste.Inventory.Application.Tests.Features.Handlers;

using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Application.Features.Handlers;
using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Identity;
using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Core.Repositories;
using Emit.Abstractions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
///     Tests observable manufacturer application handler behavior.
/// </summary>
public sealed class ManufacturerHandlersTests
{
    [Fact]
    public async Task CreateHandler_WithUniqueName_CreatesTrimmedManufacturer()
    {
        var repository = new FakeManufacturerRepository();
        var publisher = new FakeManufacturerEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUserAccessor { UserId = "alice" };
        var handler = new CreateManufacturerHandler(repository, currentUser, publisher, unitOfWork, NullLogger<CreateManufacturerHandler>.Instance);

        var response = await handler.HandleAsync(
            new CreateManufacturerCommand("  Celeste Labs  ", "contact@celeste.test", "4165551234", Utc(2026, 4, 3, 12, 0)),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Celeste Labs", response.Name);
        Assert.Equal("contact@celeste.test", response.ContactEmail);
        Assert.Equal("4165551234", response.ContactPhone);
        Assert.Single(repository.Items);
        Assert.Equal("Celeste Labs", repository.Items[0].Name);
        Assert.Equal("alice", repository.Items[0].CreatedBy);
        Assert.Equal(1, publisher.CreatedEvents.Count);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(1, unitOfWork.CommitCalls);
    }

    [Fact]
    public async Task CreateHandler_WithDuplicateName_ThrowsDuplicateManufacturerNameException()
    {
        var repository = new FakeManufacturerRepository();
        var publisher = new FakeManufacturerEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        repository.Items.Add(new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Celeste Labs",
            CreatedAt = Utc(2026, 4, 2, 12, 0),
        });

        var currentUser = new FakeCurrentUserAccessor { UserId = "alice" };
        var handler = new CreateManufacturerHandler(repository, currentUser, publisher, unitOfWork, NullLogger<CreateManufacturerHandler>.Instance);

        var action = () => handler.HandleAsync(
            new CreateManufacturerCommand("  celeste labs  ", null, null, Utc(2026, 4, 3, 12, 0)),
            CancellationToken.None);

        await Assert.ThrowsAsync<DuplicateManufacturerNameException>(action);
        Assert.Single(repository.Items);
        Assert.Equal(0, publisher.TotalPublished);
        Assert.Equal(0, unitOfWork.BeginCalls);
    }

    [Fact]
    public async Task UpdateHandler_WithExistingManufacturer_UpdatesTrimmedFields()
    {
        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            ContactEmail = "old@celeste.test",
            ContactPhone = "4165550000",
            CreatedBy = "alice",
            CreatedAt = Utc(2026, 4, 1, 8, 0),
        };

        var repository = new FakeManufacturerRepository();
        var publisher = new FakeManufacturerEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        repository.Items.Add(manufacturer);

        var currentUser = new FakeCurrentUserAccessor { UserId = "bob" };
        var handler = new UpdateManufacturerHandler(repository, currentUser, publisher, unitOfWork, NullLogger<UpdateManufacturerHandler>.Instance);

        var response = await handler.HandleAsync(
            new UpdateManufacturerCommand(manufacturer.Id, "  New Name  ", "new@celeste.test", "6475559999", Utc(2026, 4, 3, 9, 30)),
            CancellationToken.None);

        Assert.Equal(manufacturer.Id, response.Id);
        Assert.Equal("New Name", response.Name);
        Assert.Equal("new@celeste.test", manufacturer.ContactEmail);
        Assert.Equal("6475559999", manufacturer.ContactPhone);
        Assert.Equal("bob", manufacturer.LastUpdatedBy);
        Assert.Equal(Utc(2026, 4, 3, 9, 30), manufacturer.LastUpdatedAt);
        Assert.Equal("alice", manufacturer.CreatedBy);
        Assert.Equal(1, publisher.UpdatedEvents.Count);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(1, unitOfWork.CommitCalls);
        Assert.Equal(1, repository.AtomicUpdateCalls);
        Assert.Equal(0, repository.GetByIdCalls);
    }

    [Fact]
    public async Task UpdateHandler_WithDuplicateName_ThrowsDuplicateManufacturerNameException()
    {
        var repository = new FakeManufacturerRepository();
        var publisher = new FakeManufacturerEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        repository.Items.AddRange(
        [
            new Manufacturer
            {
                Id = Guid.NewGuid(),
                Name = "Northwind",
                CreatedAt = Utc(2026, 4, 1, 8, 0),
            },
            new Manufacturer
            {
                Id = Guid.NewGuid(),
                Name = "Celeste Labs",
                CreatedAt = Utc(2026, 4, 1, 8, 0),
            },
        ]);

        var currentUser = new FakeCurrentUserAccessor { UserId = "bob" };
        var handler = new UpdateManufacturerHandler(repository, currentUser, publisher, unitOfWork, NullLogger<UpdateManufacturerHandler>.Instance);

        var action = () => handler.HandleAsync(
            new UpdateManufacturerCommand(repository.Items[0].Id, "  celeste labs  ", null, null, Utc(2026, 4, 3, 9, 30)),
            CancellationToken.None);

        await Assert.ThrowsAsync<DuplicateManufacturerNameException>(action);
        Assert.Equal(0, publisher.TotalPublished);
        Assert.Equal(0, unitOfWork.BeginCalls);
        Assert.Equal(0, unitOfWork.CommitCalls);
        Assert.Equal(0, repository.AtomicUpdateCalls);
    }

    [Fact]
    public async Task UpdateHandler_WhenManufacturerMissing_ThrowsManufacturerNotFoundException()
    {
        var repository = new FakeManufacturerRepository();
        var publisher = new FakeManufacturerEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUserAccessor { UserId = "bob" };
        var handler = new UpdateManufacturerHandler(repository, currentUser, publisher, unitOfWork, NullLogger<UpdateManufacturerHandler>.Instance);

        var action = () => handler.HandleAsync(
            new UpdateManufacturerCommand(Guid.NewGuid(), "Celeste Labs", null, null, Utc(2026, 4, 3, 9, 30)),
            CancellationToken.None);

        await Assert.ThrowsAsync<ManufacturerNotFoundException>(action);
        Assert.Equal(0, publisher.TotalPublished);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(0, unitOfWork.CommitCalls);
    }

    [Fact]
    public async Task DeleteHandler_WhenManufacturerExists_SetsDeleteAuditFields()
    {
        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Celeste Labs",
            CreatedAt = Utc(2026, 4, 1, 8, 0),
        };

        var repository = new FakeManufacturerRepository();
        var publisher = new FakeManufacturerEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        repository.Items.Add(manufacturer);

        var currentUser = new FakeCurrentUserAccessor { UserId = "charlie" };
        var handler = new DeleteManufacturerHandler(repository, currentUser, publisher, unitOfWork, NullLogger<DeleteManufacturerHandler>.Instance);

        await handler.HandleAsync(
            new DeleteManufacturerCommand(manufacturer.Id, Utc(2026, 4, 4, 10, 15)),
            CancellationToken.None);

        Assert.Equal("charlie", manufacturer.DeletedBy);
        Assert.Equal(Utc(2026, 4, 4, 10, 15), manufacturer.DeletedAt);
        Assert.True(manufacturer.IsDeleted);
        Assert.Equal(1, publisher.DeletedEvents.Count);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(1, unitOfWork.CommitCalls);
        Assert.Equal(1, repository.AtomicDeleteCalls);
        Assert.Equal(0, repository.GetByIdCalls);
    }

    [Fact]
    public async Task DeleteHandler_WhenManufacturerMissing_ThrowsManufacturerNotFoundException()
    {
        var repository = new FakeManufacturerRepository();
        var publisher = new FakeManufacturerEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUserAccessor { UserId = "dana" };
        var handler = new DeleteManufacturerHandler(repository, currentUser, publisher, unitOfWork, NullLogger<DeleteManufacturerHandler>.Instance);

        var action = () => handler.HandleAsync(
            new DeleteManufacturerCommand(Guid.NewGuid(), Utc(2026, 4, 5, 11, 45)),
            CancellationToken.None);

        await Assert.ThrowsAsync<ManufacturerNotFoundException>(action);
        Assert.Equal(0, publisher.TotalPublished);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(0, unitOfWork.CommitCalls);
        Assert.Equal(1, repository.AtomicDeleteCalls);
        Assert.Equal(0, repository.GetByIdCalls);
    }

    [Fact]
    public async Task DeleteHandler_WhenManufacturerAlreadyDeleted_ThrowsManufacturerNotFoundException()
    {
        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Celeste Labs",
            CreatedAt = Utc(2026, 4, 1, 8, 0),
            DeletedBy = "charlie",
            DeletedAt = Utc(2026, 4, 4, 10, 15),
        };

        var repository = new FakeManufacturerRepository();
        var publisher = new FakeManufacturerEventPublisher();
        var unitOfWork = new FakeUnitOfWork();
        repository.Items.Add(manufacturer);

        var currentUser = new FakeCurrentUserAccessor { UserId = "dana" };
        var handler = new DeleteManufacturerHandler(repository, currentUser, publisher, unitOfWork, NullLogger<DeleteManufacturerHandler>.Instance);

        var action = () => handler.HandleAsync(
            new DeleteManufacturerCommand(manufacturer.Id, Utc(2026, 4, 5, 11, 45)),
            CancellationToken.None);

        await Assert.ThrowsAsync<ManufacturerNotFoundException>(action);
        Assert.Equal(0, publisher.TotalPublished);
        Assert.Equal("charlie", manufacturer.DeletedBy);
        Assert.Equal(Utc(2026, 4, 4, 10, 15), manufacturer.DeletedAt);
        Assert.Equal(1, unitOfWork.BeginCalls);
        Assert.Equal(0, unitOfWork.CommitCalls);
        Assert.Equal(1, repository.AtomicDeleteCalls);
        Assert.Equal(0, repository.GetByIdCalls);
    }

    [Fact]
    public async Task GetByIdHandler_WhenDeletedAndAllowed_ReturnsManufacturer()
    {
        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Celeste Labs",
            CreatedAt = Utc(2026, 4, 1, 8, 0),
            DeletedAt = Utc(2026, 4, 4, 10, 15),
        };

        var repository = new FakeManufacturerRepository();
        repository.Items.Add(manufacturer);

        var handler = new GetManufacturerByIdHandler(repository);

        var response = await handler.HandleAsync(
            new GetManufacturerByIdQuery(manufacturer.Id, true),
            CancellationToken.None);

        Assert.Equal(manufacturer.Id, response.Id);
        Assert.Equal("Celeste Labs", response.Name);
    }

    [Fact]
    public async Task GetByIdHandler_WhenDeletedAndNotAllowed_ThrowsManufacturerNotFoundException()
    {
        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Celeste Labs",
            CreatedAt = Utc(2026, 4, 1, 8, 0),
            DeletedAt = Utc(2026, 4, 4, 10, 15),
        };

        var repository = new FakeManufacturerRepository();
        repository.Items.Add(manufacturer);

        var handler = new GetManufacturerByIdHandler(repository);

        var action = () => handler.HandleAsync(
            new GetManufacturerByIdQuery(manufacturer.Id, false),
            CancellationToken.None);

        await Assert.ThrowsAsync<ManufacturerNotFoundException>(action);
    }

    [Fact]
    public async Task ListHandler_ReturnsPagedResponseAndTrimsSearchText()
    {
        var repository = new FakeManufacturerRepository();
        repository.Items.AddRange(
        [
            new Manufacturer { Id = Guid.NewGuid(), Name = "Celeste Labs", CreatedAt = Utc(2026, 4, 1, 8, 0) },
            new Manufacturer { Id = Guid.NewGuid(), Name = "Celestial Works", CreatedAt = Utc(2026, 4, 1, 8, 0) },
            new Manufacturer { Id = Guid.NewGuid(), Name = "Northern Supply", CreatedAt = Utc(2026, 4, 1, 8, 0) },
        ]);

        var handler = new ListManufacturersHandler(repository);

        var response = await handler.HandleAsync(
            new ListManufacturersQuery(1, 10, "  celes  ", false),
            CancellationToken.None);

        Assert.Equal(2, response.TotalCount);
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(2, response.Items.Count);
        Assert.Equal("CELES", repository.LastSearchText);
    }

    [Fact]
    public async Task CountHandler_WithWhitespaceSearch_PassesNullSearchText()
    {
        var repository = new FakeManufacturerRepository();
        repository.Items.Add(new Manufacturer { Id = Guid.NewGuid(), Name = "Celeste Labs", CreatedAt = Utc(2026, 4, 1, 8, 0) });
        repository.Items.Add(new Manufacturer { Id = Guid.NewGuid(), Name = "Northern Supply", CreatedAt = Utc(2026, 4, 1, 8, 0) });

        var handler = new CountManufacturersHandler(repository);

        var response = await handler.HandleAsync(
            new CountManufacturersQuery("   ", false),
            CancellationToken.None);

        Assert.Equal(2, response.TotalCount);
        Assert.Null(repository.LastCountSearchText);
    }

    private static DateTime Utc(int year, int month, int day, int hour, int minute)
    {
        return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
    }

    private sealed class FakeManufacturerRepository : IManufacturerRepository
    {
        public List<Manufacturer> Items { get; } = [];

        public string? LastSearchText { get; private set; }

        public string? LastCountSearchText { get; private set; }

        public int GetByIdCalls { get; private set; }

        public int AtomicUpdateCalls { get; private set; }

        public int AtomicDeleteCalls { get; private set; }

        public Task<long> CountAsync(string? searchText, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            LastCountSearchText = searchText;
            var count = ApplyFilter(searchText, includeDeleted).LongCount();
            return Task.FromResult(count);
        }

        public Task CreateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
        {
            Items.Add(manufacturer);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByNameAsync(string name, Guid? excludedId = null, CancellationToken cancellationToken = default)
        {
            var normalized = Normalize(name);
            var exists = Items.Any(x =>
                !x.IsDeleted &&
                x.Id != excludedId &&
                Normalize(x.Name) == normalized);

            return Task.FromResult(exists);
        }

        public Task<Manufacturer?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            GetByIdCalls++;
            var item = Items.SingleOrDefault(x => x.Id == id && (includeDeleted || !x.IsDeleted));
            return Task.FromResult(item);
        }

        public Task<IReadOnlyList<Manufacturer>> SearchAsync(string? searchText, int pageNumber, int pageSize, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            LastSearchText = searchText;
            var items = ApplyFilter(searchText, includeDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult<IReadOnlyList<Manufacturer>>(items);
        }

        public Task UpdateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<Manufacturer?> UpdateAsync(
            Guid id,
            string name,
            string? contactEmail,
            string? contactPhone,
            string? updatedBy,
            DateTime updatedAt,
            CancellationToken cancellationToken = default)
        {
            AtomicUpdateCalls++;
            var item = Items.SingleOrDefault(x => x.Id == id && !x.IsDeleted);
            if (item is null)
            {
                return Task.FromResult<Manufacturer?>(null);
            }

            item.Name = name;
            item.ContactEmail = contactEmail;
            item.ContactPhone = contactPhone;
            item.LastUpdatedBy = updatedBy;
            item.LastUpdatedAt = updatedAt;

            return Task.FromResult<Manufacturer?>(item);
        }

        public Task<Manufacturer?> DeleteAsync(Guid id, string? deletedBy, DateTime deletedAt, CancellationToken cancellationToken = default)
        {
            AtomicDeleteCalls++;
            var item = Items.SingleOrDefault(x => x.Id == id && !x.IsDeleted);
            if (item is null)
            {
                return Task.FromResult<Manufacturer?>(null);
            }

            item.DeletedBy = deletedBy;
            item.DeletedAt = deletedAt;
            return Task.FromResult<Manufacturer?>(item);
        }

        private IEnumerable<Manufacturer> ApplyFilter(string? searchText, bool includeDeleted)
        {
            return Items.Where(x => includeDeleted || !x.IsDeleted)
                .Where(x => searchText is null || Normalize(x.Name).Contains(Normalize(searchText), StringComparison.Ordinal));
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToUpperInvariant();
        }
    }

    private sealed class FakeManufacturerEventPublisher : IManufacturerEventPublisher
    {
        public List<Manufacturer> CreatedEvents { get; } = [];

        public List<Manufacturer> UpdatedEvents { get; } = [];

        public List<(Manufacturer Manufacturer, string? DeletedBy, DateTime DeletedAt)> DeletedEvents { get; } = [];

        public int TotalPublished => CreatedEvents.Count + UpdatedEvents.Count + DeletedEvents.Count;

        public Task PublishCreatedAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
        {
            CreatedEvents.Add(manufacturer);
            return Task.CompletedTask;
        }

        public Task PublishUpdatedAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
        {
            UpdatedEvents.Add(manufacturer);
            return Task.CompletedTask;
        }

        public Task PublishDeletedAsync(
            Manufacturer manufacturer,
            string? deletedBy,
            DateTime deletedAt,
            CancellationToken cancellationToken = default)
        {
            DeletedEvents.Add((manufacturer, deletedBy, deletedAt));
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
