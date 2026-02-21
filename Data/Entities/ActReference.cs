namespace gop.Data.Entities;

public class ActReference : BaseEntity
{
    public string Title { get; set; }
    public ICollection<NoticeActReference>? NoticeReferences { get; set; } = new List<NoticeActReference>();
}