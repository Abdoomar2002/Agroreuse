using Agroreuse.Domain.Entities;

namespace Agroreuse.Domain.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetByRecipientIdAsync(string recipientId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(string recipientId, CancellationToken cancellationToken = default);
    }
}
