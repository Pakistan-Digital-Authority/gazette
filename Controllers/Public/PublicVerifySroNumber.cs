using System.Net.Mime;
using gop.Requests.PublicRequests;
using gop.Services.Public;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Public;

/// <summary>
/// Controller for PublicVerifySroNumber
/// </summary>
[Route("api/public")]
[ApiVersion("1.0")]
[ApiController]
public class PublicVerifySroNumber : ControllerBase
{
    private readonly IPublicVerifySroService _service;

    /// <summary>
    /// CTOR for PublicVerifySroNumber - (public)
    /// </summary>
    /// <param name="service"></param>
    public PublicVerifySroNumber(IPublicVerifySroService service)
    {
        _service = service;
    }

    /// <summary>
    /// To verify the sro number
    /// </summary>
    /// <returns></returns>
    [HttpPost("verify-sro-number")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> VerifySroNumberAsync([FromBody] VerifySroNumberRequest request)
    {
        var response = await _service.VerifySroNumberAsync(request);
        return StatusCode(response.Status, response);
    }
}