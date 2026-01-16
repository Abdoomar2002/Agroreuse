namespace Agroreuse.Domain.Repositories;

/// <summary>
/// Unit of work interface for managing transactions.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
