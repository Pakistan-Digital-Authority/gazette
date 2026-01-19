using System.ComponentModel.DataAnnotations;

namespace gop.Requests.AuthRequests;

public class LogInRequest
{
    /// <summary>
    /// A valid email address
    /// </summary>
    [Required(ErrorMessage = "The email address field is required.")]
    public required string Email  { get; set; }

    /// <summary>
    /// For the password
    /// </summary>
    [Required(ErrorMessage = "The password field is required.")]
    public required string Password { get; set; }

    /// <summary>
    /// User Device Info
    /// </summary>
    [Required(ErrorMessage = "The device info field is required.")]
    public string DeviceInfo { get; set; }
    
    /// <summary>
    /// For Latitude
    /// </summary>
    public decimal? Latitude { get; set; }
    
    /// <summary>
    /// For Longitude
    /// </summary>
    public decimal? Longitude { get; set; }
}
