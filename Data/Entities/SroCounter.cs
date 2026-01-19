namespace gop.Data.Entities;

public class SroCounter
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public int CurrentCounter { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}