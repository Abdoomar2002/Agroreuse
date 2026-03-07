using Microsoft.Extensions.Logging;

namespace Agroreuse.Application.Services
{
    /// <summary>
    /// Mock Email service for testing. Logs emails instead of sending them.
    /// Replace with real email provider implementation (e.g., SendGrid, SMTP) when ready.
    /// </summary>
    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            // Log the email instead of actually sending it
            _logger.LogInformation("===========================================");
            _logger.LogInformation("MOCK EMAIL SERVICE - Email Details:");
            _logger.LogInformation("To: {To}", to);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {Body}", body);
            _logger.LogInformation("===========================================");

            // Simulate successful send
            return Task.FromResult(true);
        }
    }
}
