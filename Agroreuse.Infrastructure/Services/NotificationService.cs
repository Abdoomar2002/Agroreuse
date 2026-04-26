using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Enums;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly INotificationPusher? _pusher;

        public NotificationService(INotificationRepository repo, IUnitOfWork uow, INotificationPusher? pusher = null)
        {
            _repo = repo;
            _uow = uow;
            _pusher = pusher;
        }

        public async Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _repo.AddAsync(notification, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return notification;
        }

        public async Task<Notification> CreateAndSendAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _repo.AddAsync(notification, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            if (_pusher != null)
            {
                await _pusher.SendToUserAsync(notification.RecipientId, new NotificationMessage
                {
                    Id=notification.Id,
                    Title=notification.Title,
                    Message=notification.Message,
                    CreatedAt=notification.CreatedAt,
                    OrderId=notification.OrderId,
                    Status = notification.Status.ToString()
                });
            }

            return notification;
        }

        public async Task<IReadOnlyList<Notification>> GetByRecipientIdAsync(string recipientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetByRecipientIdAsync(recipientId, cancellationToken);
        }

        public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
        {
            var notification = await _repo.GetByIdAsync(notificationId, cancellationToken);
            if (notification != null)
            {
                notification.Status = NotificationStatus.Read;
                notification.ReadAt = DateTime.UtcNow;
                await _repo.UpdateAsync(notification, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<int> GetUnreadCountAsync(string recipientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetUnreadCountAsync(recipientId, cancellationToken);
        }
    }

    public class NotificationMessage
    {
        public string Status { get; set; }
        public Guid Id { get; internal set; }
        public string Title { get; internal set; }
        public string Message { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public Guid? OrderId { get; internal set; }
    }
}
