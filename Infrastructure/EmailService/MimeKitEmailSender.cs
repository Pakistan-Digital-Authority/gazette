using System;
using System.Dynamic;
using System.Threading.Tasks;
using gop.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace gop.Infrastructure.EmailService;

/// <summary>
/// MimeKit Email Sender
/// </summary>
/// <param name="configuration"></param>
public class MimeKitEmailSender(IConfiguration configuration) : IEmailSender
{
    /// <summary>
    /// Email sender async
    /// </summary>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="messageBody"></param>
    /// <returns></returns>
    public async Task<ExpandoObject> SendEmailAsync(string to, string subject, string messageBody)
    {
        dynamic response = new ExpandoObject();
        var smtpServer = configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? string.Empty);
        var senderName = configuration["EmailSettings:SenderName"];
        var senderEmail = configuration["EmailSettings:SenderEmail"];
        var smtpUsername = configuration["EmailSettings:SmtpUsername"];
        var smtpPass = configuration["EmailSettings:SmtpPassword"];
        using SmtpClient client = new SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, useSsl: true);
        await client.AuthenticateAsync(smtpUsername, smtpPass);
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;
        
        var multipart = new Multipart("alternative");
        
        // if I need unsubscribe i can add it later
        // message.Headers.Add("List-Unsubscribe", "<mailto:unsubscribe@example.com>, <https://exmaple.com/unsubscribe>");
        
        // var plainTextPart = new TextPart("plain")
        // {
        //     Text = "Some example plain text - to support text mail things - if needed"
        // };
        // multipart.Add(plainTextPart);
        
        var htmlPart =  new TextPart("html")
        {
            Text = messageBody
        };
        multipart.Add(htmlPart);

        message.Body = multipart;
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
        response.StatusCode = 200;
        response.Message = "Email sent successfully!";
        return response;
    }
}