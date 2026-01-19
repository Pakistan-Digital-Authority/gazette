using System.Dynamic;
using gop.Interfaces;

namespace gop.Infrastructure.EmailService;

/// <summary>
/// To send email - FAKE
/// </summary>
/// <param name="logger"></param>
public class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailSender
{
    /// <summary>
    /// Fake email sending service
    /// </summary>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public Task<ExpandoObject> SendEmailAsync(string to, string subject, string body)
    {
        dynamic response = new ExpandoObject();
        logger.LogInformation("Not actually sending an email to {to} - (Using Faker)", to);
        response.StatusCode = 200;
        response.Message = "Email sent successfully!";
        return response;
    }
}