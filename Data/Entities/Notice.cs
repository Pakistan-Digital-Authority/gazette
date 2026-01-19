using gop.Enums;
using gop.Interfaces;

namespace gop.Data.Entities;

/// <summary>
/// Notice Model
/// </summary>
public class Notice : BaseEntity, IAuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    /// <summary>
    /// The gazette part
    /// </summary>
    public string GazettePart { get; set; }
    /// <summary>
    /// For better search, to search the notice with the keywords
    /// </summary>
    public string? Keywords { get; set; }
    /// <summary>
    /// Either it's a blank template or an ACT/SRO type
    /// </summary>
    public NoticeTemplateTypeEnum TemplateType { get; set; } = NoticeTemplateTypeEnum.Blank;
    public string Content { get; set; }
    public string HtmlContent { get; set; }
    public string PdfUrl { get; set; }
    public DateTime PublishedDateTime { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
    /// <summary>
    /// When the notice will be effective on?
    /// </summary>
    public DateOnly EffectiveDate { get; set; }

    public int Year { get; set; }
    public string Slug { get; set; }
    public string PreviewUrl { get; set; }
    public NoticeStatusEnum Status { get; set; }
    public string SroNumber { get; set; }
    public string? Tags { get; set; }
    public ICollection<NoticeActReference> NoticeActReferences { get; set; } = new List<NoticeActReference>();
    public ICollection<Notification>? Notifications { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}