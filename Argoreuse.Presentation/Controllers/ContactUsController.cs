using Agroreuse.Application.DTOs;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly ArgoreuseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ContactUsController(
            ArgoreuseContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        /// <summary>
        /// Submit a contact us message (Authenticated users)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitContactMessage([FromBody] ContactUsRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest(new { Success = false, Message = "Message is required." });

            // Get current user from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Success = false, Message = "User not found." });

            // Create contact message
            var contactMessage = new ContactUs(
                userId,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Type,
                request.ContactType,
                request.Message
            );

            _context.ContactUsMessages.Add(contactMessage);
            await _context.SaveChangesAsync();

            // Send email notification to admin
            var emailSubject = $"New Contact Us Message - {request.ContactType}";
            var emailBody = $@"
New Contact Us Message Received

Type: {request.ContactType}
From: {user.FullName}
Email: {user.Email}
Phone: {user.PhoneNumber ?? "N/A"}
User Type: {user.Type}
Submitted At: {contactMessage.SubmittedAt:yyyy-MM-dd HH:mm:ss}

Message:
{request.Message}

---
Message ID: {contactMessage.Id}
";

            // Send email to admin (use a config value for admin email)
            await _emailService.SendEmailAsync("admin@agroreuse.com", emailSubject, emailBody);

            return Ok(new
            {
                Success = true,
                Message = "Your message has been submitted successfully. We will get back to you soon.",
                Data = new ContactUsDto
                {
                    Id = contactMessage.Id,
                    UserId = contactMessage.UserId ?? "",
                    UserName = contactMessage.UserName ?? "",
                    UserEmail = contactMessage.UserEmail ?? "",
                    UserPhone = contactMessage.UserPhone,
                    UserType = contactMessage.UserType,
                    ContactType = contactMessage.ContactType,
                    Message = contactMessage.Message ?? "",
                    SubmittedAt = contactMessage.SubmittedAt,
                    IsRead = contactMessage.IsRead
                }
            });
        }

        /// <summary>
        /// Get all contact messages (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllMessages([FromQuery] bool? isRead = null)
        {
            var query = _context.ContactUsMessages.AsQueryable();

            if (isRead.HasValue)
            {
                query = query.Where(m => m.IsRead == isRead.Value);
            }

            // Fetch raw entities first, then project to DTO in memory to handle NULLs
            var rawMessages = await query.OrderByDescending(m => m.SubmittedAt).ToListAsync();

            var messages = rawMessages.Select(m => new ContactUsDto
            {
                Id = m.Id,
                UserId = m.UserId ?? "",
                UserName = m.UserName ?? "",
                UserEmail = m.UserEmail ?? "",
                UserPhone = m.UserPhone ?? "",
                UserType = m.UserType,
                ContactType = m.ContactType,
                Message = m.Message ?? "",
                SubmittedAt = m.SubmittedAt,
                IsRead = m.IsRead,
                AdminResponse = m.AdminResponse ?? "",
                RespondedAt = m.RespondedAt
            }).ToList();

            return Ok(messages);
        }

        /// <summary>
        /// Get contact message by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetMessage(Guid id)
        {
            var message = await _context.ContactUsMessages.FindAsync(id);

            if (message == null)
                return NotFound(new { Success = false, Message = "Message not found." });

            return Ok(new ContactUsDto
            {
                Id = message.Id,
                UserId = message.UserId ?? "",
                UserName = message.UserName ?? "",
                UserEmail = message.UserEmail ?? "",
                UserPhone = message.UserPhone,
                UserType = message.UserType,
                ContactType = message.ContactType,
                Message = message.Message ?? "",
                SubmittedAt = message.SubmittedAt,
                IsRead = message.IsRead,
                AdminResponse = message.AdminResponse,
                RespondedAt = message.RespondedAt
            });
        }

        /// <summary>
        /// Mark message as read (Admin only)
        /// </summary>
        [HttpPut("{id}/mark-read")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var message = await _context.ContactUsMessages.FindAsync(id);

            if (message == null)
                return NotFound(new { Success = false, Message = "Message not found." });

            message.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Message marked as read." });
        }

        /// <summary>
        /// Respond to contact message (Admin only)
        /// </summary>
        [HttpPut("{id}/respond")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RespondToMessage(Guid id, [FromBody] string response)
        {
            if (string.IsNullOrEmpty(response))
                return BadRequest(new { Success = false, Message = "Response is required." });

            var message = await _context.ContactUsMessages.FindAsync(id);

            if (message == null)
                return NotFound(new { Success = false, Message = "Message not found." });

            message.AdminResponse = response;
            message.RespondedAt = DateTime.UtcNow;
            message.IsRead = true;
            await _context.SaveChangesAsync();

            // Send email response to user
            var emailSubject = $"Response to your {message.ContactType} message";
            var emailBody = $@"
Dear {message.UserName},

Thank you for contacting us. Here is our response to your message:

Your Original Message:
{message.Message}

Our Response:
{response}

Best regards,
Agroreuse Team
";

            await _emailService.SendEmailAsync(message.UserEmail, emailSubject, emailBody);

            return Ok(new
            {
                Success = true,
                Message = "Response sent successfully.",
                Data = new ContactUsDto
                {
                    Id = message.Id,
                    UserId = message.UserId ?? "",
                    UserName = message.UserName ?? "",
                    UserEmail = message.UserEmail ?? "",
                    UserPhone = message.UserPhone,
                    UserType = message.UserType,
                    ContactType = message.ContactType,
                    Message = message.Message ?? "",
                    SubmittedAt = message.SubmittedAt,
                    IsRead = message.IsRead,
                    AdminResponse = message.AdminResponse,
                    RespondedAt = message.RespondedAt
                }
            });
        }

        /// <summary>
        /// Get current user's contact messages
        /// </summary>
        [HttpGet("my-messages")]
        [Authorize]
        public async Task<IActionResult> GetMyMessages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var rawMessages = await _context.ContactUsMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.SubmittedAt)
                .ToListAsync();

            var messages = rawMessages.Select(m => new ContactUsDto
            {
                Id = m.Id,
                UserId = m.UserId ?? "",
                UserName = m.UserName ?? "",
                UserEmail = m.UserEmail ?? "",
                UserPhone = m.UserPhone ?? "",
                UserType = m.UserType,
                ContactType = m.ContactType,
                Message = m.Message ?? "",
                SubmittedAt = m.SubmittedAt,
                IsRead = m.IsRead,
                AdminResponse = m.AdminResponse ?? "",
                RespondedAt = m.RespondedAt
            }).ToList();

            return Ok(messages);
        }

        /// <summary>
        /// Delete contact message (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            var message = await _context.ContactUsMessages.FindAsync(id);

            if (message == null)
                return NotFound(new { Success = false, Message = "Message not found." });

            _context.ContactUsMessages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Message deleted successfully." });
        }
    }
}
