using Agroreuse.Domain.Entities;

namespace Agroreuse.Application.Services
{
    public interface INotificationService
    {
        Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default);
        Task<Notification> CreateAndSendAsync(Notification notification, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Notification>> GetByRecipientIdAsync(string recipientId, CancellationToken cancellationToken = default);
        Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(string recipientId, CancellationToken cancellationToken = default);
    }
}
