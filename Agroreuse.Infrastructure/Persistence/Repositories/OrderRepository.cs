using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Enums;
using Agroreuse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Agroreuse.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Order aggregate.
    /// </summary>
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ArgoreuseContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Order>> GetBySellerIdAsync(string sellerId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(o => o.Seller)
                .Include(o => o.Address)
                .Include(o => o.Category)
                .Include(o => o.Images)
                .Where(o => o.SellerId == sellerId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(o => o.Seller)
                .Include(o => o.Address)
                .Include(o => o.Category)
                .Include(o => o.Images)
                .Where(o => o.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(o => o.Seller)
                .Include(o => o.Address)
                .Include(o => o.Category)
                .Include(o => o.Images)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(o => o.Seller)
                .Include(o => o.Address)
                .Include(o => o.Category)
                .Include(o => o.Images)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetBySellerTypeAsync(UserType sellerType, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(o => o.Seller)
                .Include(o => o.Address)
                .Include(o => o.Category)
                .Include(o => o.Images)
                .Where(o => o.Seller.Type == sellerType)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
