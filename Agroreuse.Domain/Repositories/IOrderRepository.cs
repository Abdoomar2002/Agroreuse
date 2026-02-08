using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Enums;

namespace Agroreuse.Domain.Repositories
{
    /// <summary>
    /// Repository interface specific to Order aggregate.
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetBySellerIdAsync(string sellerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    }
}
