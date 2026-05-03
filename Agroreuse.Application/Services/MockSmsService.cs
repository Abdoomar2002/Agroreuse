using Microsoft.Extensions.Logging;

namespace Agroreuse.Application.Services
{
    /// <summary>
    /// Mock SMS service for testing. Logs SMS messages instead of sending them.
    /// Replace with real SMS provider implementation (e.g., Twilio, AWS SNS) when ready.
    /// </summary>
    public class MockSmsService : ISmsService
    {
        private readonly ILogger<MockSmsService> _logger;

        public MockSmsService(ILogger<MockSmsService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            // Log the SMS instead of actually sending it
            _logger.LogInformation("===========================================");
            _logger.LogInformation("MOCK SMS SERVICE - Message Details:");
            _logger.LogInformation("To: {PhoneNumber}", phoneNumber);
            _logger.LogInformation("Message: {Message}", message);
            _logger.LogInformation("===========================================");

            // Simulate successful send
            return Task.FromResult(true);
        }

        public Task<bool> SendOtpAsync(string phoneNumber, string otp)
        {
            _logger.LogInformation("===========================================");
            _logger.LogInformation("MOCK SMS SERVICE - OTP Details:");
            _logger.LogInformation("To: {PhoneNumber}", phoneNumber);
            _logger.LogInformation("OTP: {Otp}", otp);
            _logger.LogInformation("===========================================");

            return Task.FromResult(true);
        }
    }
}
