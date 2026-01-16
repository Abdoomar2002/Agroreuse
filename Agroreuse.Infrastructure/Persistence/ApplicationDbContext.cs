using Agroreuse.Domain.Common;
using Agroreuse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agroreuse.Infrastructure.Persistence;

/// <summary>
/// Application database context.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SampleEntity> SampleEntities => Set<SampleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<AggregateRoot>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(AggregateRoot.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
