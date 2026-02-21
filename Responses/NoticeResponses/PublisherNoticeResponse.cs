using gop.Enums;

namespace gop.Responses.NoticeResponses;

/// <summary>
/// To show the notice details
/// </summary>
public class PublisherNoticeResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string HtmlContent { get; set; }
    public string Description { get; set; }
    public string SroNumber { get; set; }
    public string GazettePart { get; set; }
    public string Ministry { get; set; }
    public string IssuingAuthorityName { get; set; }
    public string? IssuingAuthorityDesignation { get; set; }
    public string? Tags { get; set; }
    public string PreviewUrl { get; set; }
    public string PdfUrl { get; set; }
    public NoticeStatusEnum Status { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public DateTime PublishedDateTime { get; set; }
    public List<RelatedActListResponse>? ActReference { get; set; }
}


/// <summary>
/// The related act details
/// </summary>
public class RelatedActListResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
}

/// <summary>
/// Publisher Notice Tags
/// </summary>
public class PublisherNoticeTagResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
}