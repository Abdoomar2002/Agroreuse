using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Agroreuse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for SampleEntity.
/// </summary>
public class SampleEntityRepository : Repository<SampleEntity>, ISampleEntityRepository
{
    public SampleEntityRepository(ArgoreuseContext context) : base(context)
    {
    }

    public async Task<SampleEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<SampleEntity>> GetActiveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.IsActive).ToListAsync(cancellationToken);
    }
}
