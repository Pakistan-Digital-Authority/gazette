namespace gop.Responses.AdminResponses;

/// <summary>
/// SRO RULE
/// </summary>
public class GetSroRuleResponse
{
    public string SroPatternFormat { get; set; }
    public int CurrentCounter { get; set; }
    public int NextAvailable { get; set; }
}