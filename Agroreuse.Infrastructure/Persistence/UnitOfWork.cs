using Agroreuse.Domain.Repositories;

namespace Agroreuse.Infrastructure.Persistence;

/// <summary>
/// Unit of work implementation.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ArgoreuseContext _context;

    public UnitOfWork(ArgoreuseContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
