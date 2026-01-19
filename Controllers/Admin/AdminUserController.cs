using System.Net.Mime;
using gop.Enums;
using gop.Requests.AdminRequests;
using gop.Services.AdminServices;
using gop.Utilities;
using gop.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Admin;

/// <summary>
/// Controller for notices - management
/// </summary>
[Route("api/admin-users")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class AdminUserController : ControllerBase
{
    private readonly IAdminUserService _service;

    /// <summary>
    /// CTOR for admin user controller
    /// </summary>
    /// <param name="service"></param>
    public AdminUserController(IAdminUserService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// List users (Admin)
    /// </summary>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> GetUsersAsync([FromQuery] AdminGetUserListRequest request)
    {
        var response = await _service.GetUsersAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// Get user by id
    /// </summary>
    [HttpGet("{userId:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> GetUserByIdAsync(Guid userId)
    {
        var response = await _service.GetUserByIdAsync(userId);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// To add a new user - admin
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> CreateUserAsync([FromBody] AdminCreateUserRequest request)
    {
        var response = await _service.CreateUserAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{userId:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> UpdateUserAsync(
        Guid userId,
        [FromBody] AdminUpdateUserRequest request)
    {
        var response = await _service.UpdateUserAsync(userId, request);
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Resend invitation email on the specified user email address
    /// </summary>
    /// <response code="200">Returns the success scenario.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If something goes wrong on the server.</response>
    [HttpPost("resend-invitation")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> ResendInvitationEmailAsync(ResendInvitationEmailRequest request)
    {
        var response = await _service.ResendInvitationEmailAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{userId:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Admin)]
    public async Task<IActionResult> DeleteUserAsync(Guid userId)
    {
        var response = await _service.DeleteUserAsync(userId);
        return StatusCode(response.Status, response);
    }
}