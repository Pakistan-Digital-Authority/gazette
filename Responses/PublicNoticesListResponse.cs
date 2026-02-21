namespace gop.Responses;

/// <summary>
/// For public Notices List
/// </summary>
public class PublicNoticesListResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string GazettePart { get; set; }
    public string Ministry { get; set; }
    public string SroNumber { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? Tags { get; set; }
    public string PublishedDateTime { get; set; }
}