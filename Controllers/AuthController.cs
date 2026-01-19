using System.Net.Mime;
using gop.Requests.AuthRequests;
using gop.Services;
using gop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers;

/// <summary>
/// Controller for authentication - login - register - etc.
/// </summary>
[Route("api/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="service"></param>
    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>
    /// Performs authentication - a quick login
    /// </summary>
    /// <remarks>
    /// A sample request payload for authentication.:
    ///
    ///     POST /authenticate
    ///     {
    ///         "email": "admin@gop.gov.pk",
    ///         "password": "P@ssw0rd",
    ///         "deviceInfo": "Chrome 143 on Windows 10 (Desktop)"
    ///     }
    /// </remarks>
    /// <param name="request">Email Address and password.</param>
    /// <response code="200">Returns the access token.</response>
    /// <response code="400">Returns a list of errors if the request is invalid.</response>
    /// <response code="404">When no account is found for the provided email and password.</response>
    [HttpPost("authenticate")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Authenticate([FromBody] LogInRequest request)
    {
        var response = await _service.AuthenticateAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// Performs Refresh Token
    /// </summary>
    /// <remarks>
    /// A sample request payload for refresh token.:
    ///
    ///     POST /refresh-token
    ///     {
    ///         "token": "some-token-here",
    ///     }
    /// </remarks>
    /// <param name="request">token.</param>
    /// <response code="200">Returns the access token.</response>
    /// <response code="400">Returns a list of errors if the request is invalid.</response>
    /// <response code="404">When no account is found for the provided email and password.</response>
    [HttpPost("refresh-token")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var response = await _service.RefreshTokenAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// Performs Change Password
    /// </summary>
    /// <remarks>
    /// A sample request payload for change password.:
    ///
    ///     POST /change-password
    ///     {
    ///         "email": "shakil.khan@entropy-x.com",
    ///     }
    /// </remarks>
    /// <param name="request">email</param>
    /// <response code="200">Returns the change password success message.</response>
    /// <response code="400">Returns a list of errors if the request is invalid.</response>
    /// <response code="404">When no account is found for the provided email and password.</response>
    [HttpPost("forgot-password")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ForgotPassword([FromBody] ChangePasswordRequest request)
    {
        var response = await _service.SendResetPasswordEmailAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// Performs AD Account Change Password (NOT WORKING FOR NOW)
    /// </summary>
    /// <remarks>
    /// A sample request payload for change password.:
    ///
    ///     POST /change-password
    ///     {
    ///         "token": "some-token",
    ///         "newPassword": "P@ssw0rd",
    ///         "confirmPassword": "newP@ssw0rd"
    ///     }
    /// </remarks>
    /// <param name="request">token password, confirm password.</param>
    /// <response code="200">Returns the change password success message.</response>
    /// <response code="400">Returns a list of errors if the request is invalid.</response>
    /// <response code="404">When no account is found for the provided email and password.</response>
    [HttpPost("reset-password")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var response = await _service.ResetPasswordAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// Logout & Revoke the token
    /// </summary>
    /// <response code="200">Returns the logout response.</response>
    /// <response code="400">Returns a list of errors if the request is invalid.</response>
    /// <response code="404">When no account is found against the provided token.</response>
    [HttpGet("log-out")]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> LogoutAsync()
    {
        var response = await _service.LogoutAsync();
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// Get User Profile
    /// </summary>
    /// <response code="200">Returns the user details in response.</response>
    /// <response code="400">Returns a list of errors if the request is invalid.</response>
    /// <response code="404">When no account is found against the provided token.</response>
    [HttpGet("profile")]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ProfileAsync()
    {
        var response = await _service.ProfileAsync();
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Accept the joining invitation
    /// </summary>
    /// <response code="200">Returns the success scenario.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If something goes wrong on the server.</response>
    [HttpPost("accept-invitation")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<IActionResult> AcceptInvitation(AcceptInvitationRequest request)
    {
        var response = await _service.AcceptInvitationAsync(request);
        return StatusCode(response.Status, response);
    }
}