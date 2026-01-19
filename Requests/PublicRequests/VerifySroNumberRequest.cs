using System.ComponentModel.DataAnnotations;

namespace gop.Requests.PublicRequests;

/// <summary>
/// To verify the sro number
/// </summary>
public class VerifySroNumberRequest
{
    /// <summary>
    /// The sro number
    /// </summary>
    [Required]
    public string SroNumber { get; set; }
}