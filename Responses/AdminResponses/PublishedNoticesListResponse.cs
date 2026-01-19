namespace gop.Responses.AdminResponses;

/// <summary>
/// List of published notices
/// </summary>
public class PublishedNoticesListResponse
{
    public Guid Id { get; set; }
    public string Ministry { get; set; }
    public string Title { get; set; }
    public string SroNumber { get; set; }
    public DateTime PublishedDate { get; set; }
    public string Status { get; set; }
}