using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Agroreuse.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Address entity.
    /// </summary>
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(ArgoreuseContext context) : base(context)
        {
        }

        /// <summary>
        /// Get address by user ID (profile address)
        /// </summary>
        public async Task<Address?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(a => a.ApplicationUserId == userId, cancellationToken);
        }
    }
}
