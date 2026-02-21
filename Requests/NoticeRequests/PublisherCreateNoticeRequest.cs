using System.ComponentModel.DataAnnotations;
using gop.Enums;

namespace gop.Requests.NoticeRequests;

/// <summary>
/// A payload to create notices
/// </summary>
public class PublisherCreateNoticeRequest
{
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
    [Required]
    public DateOnly EffectiveDate { get; set; }
    public NoticeStatusEnum Status { get; set; }
    public List<string>? RelatedActs { get; set; }
}