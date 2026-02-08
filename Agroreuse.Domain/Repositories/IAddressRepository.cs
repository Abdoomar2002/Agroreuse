using Agroreuse.Domain.Entities;

namespace Agroreuse.Domain.Repositories
{
    /// <summary>
    /// Repository interface specific to Address entity.
    /// </summary>
    public interface IAddressRepository : IRepository<Address>
    {
        /// <summary>
        /// Get address by user ID (profile address)
        /// </summary>
        Task<Address?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
