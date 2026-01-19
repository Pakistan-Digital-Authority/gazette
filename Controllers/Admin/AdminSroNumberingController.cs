using System.Net.Mime;
using gop.Enums;
using gop.Services.AdminServices;
using gop.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Admin;

/// <summary>
/// Controller for SRO Formatting - Details
/// </summary>
[Route("api/admin-sro-numbering")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class AdminSroNumberingController : ControllerBase
{
    private readonly IAdminUserService _service;

    /// <summary>
    /// CTOR for admin sro numbering
    /// </summary>
    /// <param name="service"></param>
    public AdminSroNumberingController(IAdminUserService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// List users (Admin)
    /// </summary>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> GetSroRuleAsync()
    {
        var response = await _service.GetSroRuleAsync();
        return StatusCode(response.Status, response);
    }
}