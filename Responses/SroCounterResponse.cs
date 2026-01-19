namespace gop.Responses;

/// <summary>
/// SRO Numbering Response
/// </summary>
public class SroCounterResponse
{
    public string Numbering { get; set; }
    public int Year { get; set; }
    public int CurrentCounter { get; set; }
    public string Slug { get; set; }
}