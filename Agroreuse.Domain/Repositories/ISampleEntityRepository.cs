using Agroreuse.Domain.Entities;

namespace Agroreuse.Domain.Repositories;

/// <summary>
/// Repository interface specific to SampleEntity aggregate.
/// </summary>
public interface ISampleEntityRepository : IRepository<SampleEntity>
{
    Task<SampleEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SampleEntity>> GetActiveEntitiesAsync(CancellationToken cancellationToken = default);
}
