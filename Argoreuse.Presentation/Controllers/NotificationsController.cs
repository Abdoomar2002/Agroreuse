using Agroreuse.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get all notifications for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized(new { Success = false, Message = "User not authenticated." });

            var notifications = await _notificationService.GetByRecipientIdAsync(userId);

            var result = notifications.Select(n => new
            {
                n.Id,
                n.Title,
                n.Message,
                n.CreatedAt,
                n.ReadAt,
                n.OrderId,
                Status = n.Status.ToString(),
                IsRead = n.Status == Agroreuse.Domain.Enums.NotificationStatus.Read
            });

            return Ok(new
            {
                Success = true,
                Data = result,
                Message = "Notifications retrieved successfully."
            });
        }

        /// <summary>
        /// Get unread notification count for the current user
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized(new { Success = false, Message = "User not authenticated." });

            var count = await _notificationService.GetUnreadCountAsync(userId);

            return Ok(new
            {
                Success = true,
                Data = new { UnreadCount = count },
                Message = "Unread count retrieved successfully."
            });
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id);

            return Ok(new
            {
                Success = true,
                Message = "Notification marked as read."
            });
        }

        /// <summary>
        /// Mark all notifications as read for the current user
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized(new { Success = false, Message = "User not authenticated." });

            var notifications = await _notificationService.GetByRecipientIdAsync(userId);
            foreach (var notification in notifications.Where(n => n.Status != Agroreuse.Domain.Enums.NotificationStatus.Read))
            {
                await _notificationService.MarkAsReadAsync(notification.Id);
            }

            return Ok(new
            {
                Success = true,
                Message = "All notifications marked as read."
            });
        }
    }
}
