using System.Net.Mime;
using gop.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Admin;

/// <summary>
/// Controller for Analytics - Admin
/// </summary>
[Route("api/admin")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class AdminAnalyticController : ControllerBase
{
    private readonly IAdminAnalyticsService _service;
    /// <summary>
    /// CTOR for AdminAnalytic
    /// </summary>
    /// <param name="service"></param>
    public AdminAnalyticController(IAdminAnalyticsService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// List dashboard analytics for admin
    /// </summary>
    [HttpGet("summary")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAdminDashboardAnalyticsAsync()
    {
        var response = await _service.GetAdminDashboardAnalyticsAsync();
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// List analytics for admin
    /// </summary>
    [HttpGet("analytics")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAdminAnalyticsAsync()
    {
        var response = await _service.GetAdminAnalyticsAsync();
        return StatusCode(response.Status, response);
    }
}