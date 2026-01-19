using System.Net.Mime;
using gop.Enums;
using gop.Requests.NoticeRequests;
using gop.Requests.PublicRequests;
using gop.Services.Public;
using gop.Services.Publisher;
using gop.Validators;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Public;

/// <summary>
/// Controller for PublicHomeController
/// </summary>
[Route("api/public")]
[ApiVersion("1.0")]
[ApiController]
public class PublicHomeController : ControllerBase
{
    private readonly IPublicNoticeService _service;

    /// <summary>
    /// CTOR for PublicHomeController - (public)
    /// </summary>
    /// <param name="service"></param>
    public PublicHomeController(IPublicNoticeService service)
    {
        _service = service;
    }

    /// <summary>
    /// To get all the notices/gazettes
    /// </summary>
    /// <returns></returns>
    [HttpGet("notices")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAllPublishedAsync([FromQuery] PublicNoticesGetListRequest request)
    {
        var response = await _service.GetAllAsync(request);
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// To get notice by ID
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var response = await _service.GetByIdAsync(id);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// To get notice file url by notice ID
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <returns></returns>
    [HttpGet("notice-pdf-file/{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetNoticeFileUrlAsync(Guid id)
    {
        var response = await _service.GetNoticePdfFiledAsync(id);
        return StatusCode(response.Status, response);
    }
}