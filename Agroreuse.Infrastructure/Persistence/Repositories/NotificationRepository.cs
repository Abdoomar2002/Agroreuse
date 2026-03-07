using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Enums;
using Agroreuse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Agroreuse.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(ArgoreuseContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Notification>> GetByRecipientIdAsync(string recipientId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(n => n.RecipientId == recipientId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountAsync(string recipientId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .CountAsync(n => n.RecipientId == recipientId && n.Status != NotificationStatus.Read, cancellationToken);
        }
    }
}
