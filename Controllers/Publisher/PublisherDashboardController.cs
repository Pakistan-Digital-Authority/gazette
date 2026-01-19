using System.Net.Mime;
using gop.Enums;
using gop.Services.Publisher;
using gop.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Publisher;

/// <summary>
/// Controller for notices - management
/// </summary>
[Route("api/publisher-dashboard")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class PublisherDashboardController : ControllerBase
{
    private readonly INoticeService _service;

    /// <summary>
    /// CTOR for PublisherDashboard - (publisher)
    /// </summary>
    /// <param name="service"></param>
    public PublisherDashboardController(INoticeService service)
    {
        _service = service;
    }

    /// <summary>
    /// To get all the notices of the publisher - Only published
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> GetDashboardAnalyticsAsync()
    {
        var response = await _service.GetDashboardAnalyticsAsync();
        return StatusCode(response.Status, response);
    }
}