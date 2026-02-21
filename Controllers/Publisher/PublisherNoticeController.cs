using System.Net.Mime;
using gop.Enums;
using gop.Requests.AuthRequests;
using gop.Requests.NoticeRequests;
using gop.Requests.PublicRequests;
using gop.Services.Publisher;
using gop.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gop.Controllers.Publisher;

/// <summary>
/// Controller for notices - management
/// </summary>
[Route("api/publisher-notices")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class PublisherNoticeController : ControllerBase
{
    private readonly INoticeService _service;

    /// <summary>
    /// CTOR for notice controller - (publisher)
    /// </summary>
    /// <param name="service"></param>
    public PublisherNoticeController(INoticeService service)
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
    public async Task<IActionResult> GetAllAsync([FromQuery] GetPublisherNoticesListRequest request)
    {
        var response = await _service.GetAllAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// To get all the notices of the publisher - Only Draft
    /// </summary>
    /// <returns></returns>
    [HttpGet("drafts")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> GetAllDraftsAsync([FromQuery] GetPublisherNoticesListRequest request)
    {
        var response = await _service.GetAllDraftsAsync(request);
        return StatusCode(response.Status, response);
    }
    
    /// <summary>
    /// To search and get all the notices of the publisher type ministry - Only published
    /// </summary>
    /// <returns></returns>
    [HttpGet("search")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> GetSearchQueryResultAsync([FromQuery] PublicNoticesGetListRequest request)
    {
        var response = await _service.GetSearchQueryResultAsync(request);
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
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var response = await _service.GetByIdAsync(id);
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// To create a new notice
    /// </summary>
    /// <param name="request">Notice creation data</param>
    /// <returns></returns>
    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [Produces(MediaTypeNames.Application.Json)]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> CreateAsync([FromForm] PublisherCreateNoticeRequest request)
    {
        var response = await _service.CreateAsync(request);
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// To update an existing notice
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <param name="request">Notice update data</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [Produces(MediaTypeNames.Application.Json)]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromForm] PublisherUpdateNoticeRequest request)
    {
        var response = await _service.UpdateAsync(id, request);
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// To delete a notice by ID
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [HasPermission(UserRoleEnum.Publisher)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var response = await _service.DeleteAsync(id);
        return StatusCode(response.Status, response);
    }
}