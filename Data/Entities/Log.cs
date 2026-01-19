using System.ComponentModel.DataAnnotations;
using gop.Enums;

namespace gop.Data.Entities;

/// <summary>
/// System Logs for GOT - API
/// </summary>
public class Log
{
    /// <summary>
    /// Primary Key
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Log level (Info, Warning, Error, Critical)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public LogLevelEnum Level { get; set; } = LogLevelEnum.Info;

    /// <summary>
    /// To which type of logs are they
    /// </summary>
    public LogTypeEnum LogType { get; set; } = LogTypeEnum.General;

    /// <summary>
    /// Log title (short summary)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Title { get; set; }

    /// <summary>
    /// Full exception or detailed message (stack trace etc.)
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Source of the log (Controller, Service, Job, Middleware)
    /// </summary>
    [MaxLength(150)]
    public string? Source { get; set; }

    /// <summary>
    /// Action or operation name
    /// </summary>
    [MaxLength(150)]
    public string? Action { get; set; }

    /// <summary>
    /// HTTP method (GET, POST, PUT, DELETE)
    /// </summary>
    [MaxLength(10)]
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Request path or endpoint
    /// </summary>
    [MaxLength(300)]
    public string? RequestPath { get; set; }

    /// <summary>
    /// HTTP status code (200, 400, 500 etc.)
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Logged-in user id (if available)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Logged-in user email or username (snapshot)
    /// </summary>
    [MaxLength(256)]
    public string? UserEmail { get; set; }

    /// <summary>
    /// Client IP address
    /// </summary>
    [MaxLength(50)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Client user agent
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Metadata for advanced/custom json things
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When the log was created
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}