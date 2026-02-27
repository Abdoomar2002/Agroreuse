using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IUnitOfWork _uow;

        public NotificationService(INotificationRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _repo.AddAsync(notification, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return notification;
        }
    }
}
