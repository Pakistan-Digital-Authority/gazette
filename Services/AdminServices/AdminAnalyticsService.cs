using gop.Repositories.AdminRepos;
using gop.Responses.AdminResponses;
using gop.Utilities;

namespace gop.Services.AdminServices;

/// <summary>
/// Admin Analytics Service Interface
/// </summary>
public interface IAdminAnalyticsService
{
    /// <summary>
    /// Get dashboard summary analytics for admin
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<AdminDashboardAnalyticsResponse>> GetAdminDashboardAnalyticsAsync();

    /// <summary>
    /// Get detailed analytics for admin
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<AdminAnalyticsResponse>> GetAdminAnalyticsAsync();
}

/// <summary>
    /// Admin Analytics Service Implementation
    /// </summary>
    public class AdminAnalyticsService : IAdminAnalyticsService
    {
        private readonly IAdminAnalyticsRepo _repo;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="repo"></param>
        public AdminAnalyticsService(IAdminAnalyticsRepo repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Fetch dashboard summary analytics
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse<AdminDashboardAnalyticsResponse>> GetAdminDashboardAnalyticsAsync()
        {
            var response = new ApiResponse<AdminDashboardAnalyticsResponse>();
            try
            {
                var data = await _repo.GetDashboardSummaryAsync();

                if (data is null)
                {
                    response.Status = 404;
                    response.Message = "Dashboard analytics not found.";
                    return response;
                }

                response.Status = 200;
                response.Message = "Dashboard analytics fetched successfully.";
                response.Data = data;
                return response;
            }
            catch (Exception ex)
            {
                response.Status = 500;
                response.Message = ex.Message;
                return response;
            }
        }

        /// <summary>
        /// Fetch detailed analytics data
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse<AdminAnalyticsResponse>> GetAdminAnalyticsAsync()
        {
            var response = new ApiResponse<AdminAnalyticsResponse>();
            try
            {
                var data = await _repo.GetDetailedAnalyticsAsync();

                if (data == null)
                {
                    response.Status = 404;
                    response.Message = "Analytics data not found.";
                    return response;
                }

                response.Status = 200;
                response.Message = "Analytics fetched successfully.";
                response.Data = data;
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