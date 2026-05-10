using Agroreuse.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;

    // Use mail5015.site4now.net for SSL, or mail.sifsafeg.com for plain
    private const string SmtpHost = "mail5015.site4now.net";
    private const int SmtpPort = 465;
    private const string SmtpUser = "no-reply@sifsafeg.com";
    private const string SmtpPass = "SamiraAhmed@2026";
    private const string FromEmail = "no-reply@sifsafeg.com";

    public SmtpEmailService(ILogger<SmtpEmailService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(SmtpHost, SmtpPort, SecureSocketOptions.SslOnConnect); // for 465
            await client.AuthenticateAsync(SmtpUser, SmtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return false;
        }
    }
}