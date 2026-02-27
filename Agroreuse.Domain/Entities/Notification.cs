using Agroreuse.Domain.Common;
using Agroreuse.Domain.Enums;

namespace Agroreuse.Domain.Entities
{
    /// <summary>
    /// Notification entity to store notifications for users.
    /// </summary>
    public class Notification : Entity
    {
        public string RecipientId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public NotificationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }

        public Guid? OrderId { get; set; }

        public Notification()
        {
            CreatedAt = DateTime.UtcNow;
            Status = NotificationStatus.Created;
        }
    }
}
