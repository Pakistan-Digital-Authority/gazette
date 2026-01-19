using System.Net.Mime;
using gop.Enums;
using gop.Requests;
using gop.Services.Publisher;
using gop.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Publisher;

/// <summary>
/// Controller for publisher notifications
/// </summary>
[Route("api/publisher-notifications")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class PublisherNotificationsController : ControllerBase
{
    private readonly IPublisherNotificationService _service;

    /// <summary>
    /// CTOR for publisher notifications controller
    /// </summary>
    /// <param name="service"></param>
    public PublisherNotificationsController(IPublisherNotificationService service)
    {
        _service = service;
    }

    /// <summary>
    /// To get all notifications for the publisher
    /// </summary>
    /// <param name="request">Pagination / filter data</param>
    /// <returns></returns>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetPublisherNotificationsRequest request)
    {
        var response = await _service.GetAllAsync(request);
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// To mark a notification as read
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <returns></returns>
    [HttpPut("{id}/read")]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> MarkAsReadAsync(Guid id)
    {
        var response = await _service.MarkAsReadAsync(id);
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// To mark all notifications as read
    /// </summary>
    /// <returns></returns>
    [HttpPut("read-all")]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> MarkAllAsReadAsync()
    {
        var response = await _service.MarkAllAsReadAsync();
        return StatusCode(response.Status, response);
    }
}
