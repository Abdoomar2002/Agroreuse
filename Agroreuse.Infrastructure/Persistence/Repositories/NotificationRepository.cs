using Agroreuse.Domain.Entities;
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
            return await DbSet.Where(n => n.RecipientId == recipientId).ToListAsync(cancellationToken);
        }
    }
}
