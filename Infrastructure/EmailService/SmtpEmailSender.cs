using System.Dynamic;
using System.Net;
using System.Net.Mail;
using gop.Interfaces;

namespace gop.Infrastructure.EmailService;

/// <summary>
/// SMTP Email service - implementation
/// </summary>
/// <param name="_logger"></param>
/// <param name="configuration"></param>
public class SmtpEmailSender(ILogger<SmtpEmailSender> _logger, IConfiguration configuration) : IEmailSender
{
    /// <summary>
    /// To send email async
    /// </summary>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<ExpandoObject> SendEmailAsync(string to, string subject, string body)
    {
        dynamic response = new ExpandoObject();
        try
        {
            var smtpServer = configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? string.Empty);
            var senderName = configuration["EmailSettings:SenderName"];
            var senderEmail = configuration["EmailSettings:SenderEmail"];
            var smtpUsername = configuration["EmailSettings:SmtpUsername"];
            var smtpPass = configuration["EmailSettings:SmtpPassword"];

            var emailClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPass),
                EnableSsl = true
            };
            var message = new MailMessage { From = new MailAddress(senderEmail!), Subject = subject, Body = body, IsBodyHtml = true};
            message.To.Add(new MailAddress(to));
            await emailClient.SendMailAsync(message);

            _logger.LogWarning("Sending email to {to} - (Using SMTP)", to);

            response.StatusCode = 200;
            response.Message = "Email sent successfully!";
            return response;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return response;
        }
    }
}
