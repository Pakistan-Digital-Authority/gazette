using System.Net.Mime;
using gop.Enums;
using gop.Requests.AdminRequests;
using gop.Services.AdminServices;
using gop.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Admin;

/// <summary>
/// Controller for published notices - activity
/// </summary>
[Route("api/admin-overview")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class AdminOverviewController : ControllerBase
{
    private readonly IAdminPublishedService _service;
    /// <summary>
    /// CTOR for Admin published activity controller
    /// </summary>
    /// <param name="service"></param>
    public AdminOverviewController(IAdminPublishedService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// List Published Notices - (Admin)
    /// </summary>
    [HttpGet("notices")]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> GetPublishedNoticesAsync([FromQuery] PublishedNoticesListRequest request)
    {
        var response = await _service.GetPublishedNoticesAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// List recent activities - (Admin)
    /// </summary>
    [HttpGet("activities")]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> GetRecentActivitiesAsync([FromQuery] AdminRecentActivitiesRequest request)
    {
        var response = await _service.GetRecentActivitiesAsync(request);
        return StatusCode(response.Status, response);
    }
}