using System.Dynamic;

namespace gop.Interfaces;

/// <summary>
/// For email sender - interface
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Will send the email
    /// </summary>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    Task<ExpandoObject> SendEmailAsync(string to, string subject, string body);
}