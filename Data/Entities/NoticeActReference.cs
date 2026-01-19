namespace gop.Data.Entities;

public class NoticeActReference
{
    public Guid Id { get; set; }
    public Guid NoticeId { get; set; }
    public Notice Notice { get; set; }
    public Guid ActId { get; set; }
    public ActReference ActReference { get; set; }
}