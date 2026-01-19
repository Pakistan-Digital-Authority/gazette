using System.ComponentModel.DataAnnotations;
using gop.Enums;

namespace gop.Requests.NoticeRequests;

/// <summary>
/// A payload to update notices
/// </summary>
public class PublisherUpdateNoticeRequest
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string GazettePart { get; set; }
    public string? Keywords { get; set; }
    public string? Tags { get; set; }
    [Required]
    public NoticeTemplateTypeEnum TemplateType { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public string HtmlContent { get; set; }
    [Required]
    public IFormFile PdfContent { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public NoticeStatusEnum Status { get; set; }
}