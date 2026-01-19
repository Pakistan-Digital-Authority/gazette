using gop.Repositories.GeneralRepos;
using gop.Requests.AdminRequests;
using gop.Responses.AdminResponses;
using gop.Utilities;

namespace gop.Services.AdminServices;

/// <summary>
/// Audit and Logs Interface
/// </summary>
public interface IAdminAuditAndLogService
{
    /// <summary>
    /// Response for audit & Logs
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse<PagedResult<AuditAndLogListResponse>>> GetAuditAndLogsAsync(AdminAuditAndLogRequest request);
}

/// <summary>
/// Audit and logs service - admin
/// </summary>
public class AdminAuditAndLogService : IAdminAuditAndLogService
{
    private readonly ILogsRepo _repo;

    /// <summary>
    /// Logs CTOR
    /// </summary>
    /// <param name="repo"></param>
    public AdminAuditAndLogService(ILogsRepo repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Audit And Logs Service
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse<PagedResult<AuditAndLogListResponse>>> GetAuditAndLogsAsync(AdminAuditAndLogRequest request)
    {
        var response = new ApiResponse<PagedResult<AuditAndLogListResponse>>();
        try
        {
            var logs = await _repo.GetPagedLogsAsync(request.PageNumber, request.PageSize, "audit", request.Search);

            if (!logs.Items.Any())
            {
                response.Status = 404;
                response.Message = "No logs found.";
                return response;
            }
            
            var result = logs.Items.Select(log => new AuditAndLogListResponse
            {
                Id = log.Id,
                CreatedAt = log.CreatedAt,
                Status = log.Action,
                Title = log.Title,
                Message = log.Message,
            }).ToList();
            
            var pagedResponse = new PagedResult<AuditAndLogListResponse>(result, logs.TotalCount, logs.PageNumber, logs.PageSize);
            
            response.Status = 200;
            response.Message = "Logs fetched successfully!";
            response.Data = pagedResponse;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }
}