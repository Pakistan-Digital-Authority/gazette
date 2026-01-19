using System.Net.Mime;
using gop.Enums;
using gop.Requests.AdminRequests;
using gop.Services.AdminServices;
using gop.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Admin;

/// <summary>
/// Controller for Audit & Logs
/// </summary>
[Route("api/admin-audit-and-logs")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class AdminAuditAndLogController : Controller
{
    private readonly IAdminAuditAndLogService _service;
    /// <summary>
    /// CTOR for Admin AdminAuditAndLogs
    /// </summary>
    /// <param name="service"></param>
    public AdminAuditAndLogController(IAdminAuditAndLogService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// List Audit & Logs for admin
    /// </summary>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> GetAuditAndLogsAsync([FromQuery] AdminAuditAndLogRequest request)
    {
        var response = await _service.GetAuditAndLogsAsync(request);
        return StatusCode(response.Status, response);
    }
}