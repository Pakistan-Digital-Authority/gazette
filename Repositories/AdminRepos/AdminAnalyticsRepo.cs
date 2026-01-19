using gop.Data;
using gop.Enums;
using gop.Responses.AdminResponses;
using Microsoft.EntityFrameworkCore;

namespace gop.Repositories.AdminRepos;

/// <summary>
/// Interface for admin analytics
/// </summary>
public interface IAdminAnalyticsRepo
{
    /// <summary>
    /// To get the dashboard summary
    /// </summary>
    /// <returns></returns>
    Task<AdminDashboardAnalyticsResponse> GetDashboardSummaryAsync();
    
    /// <summary>
    /// to get the details analytics - for admin only
    /// </summary>
    /// <returns></returns>
    Task<AdminAnalyticsResponse> GetDetailedAnalyticsAsync();
}

/// <summary>
/// THE REPO for analytics
/// </summary>
public class AdminAnalyticsRepo : IAdminAnalyticsRepo
    {
        private readonly DatabaseContext _context;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="context"></param>
        public AdminAnalyticsRepo(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// To get the dashboard summary
        /// </summary>
        /// <returns></returns>
        public async Task<AdminDashboardAnalyticsResponse> GetDashboardSummaryAsync()
        {
            var now = DateTime.UtcNow;
            var startCurrentPeriod = now.AddDays(-30);
            var startPreviousPeriod = now.AddDays(-60);
            var endPreviousPeriod = now.AddDays(-31);
            
            var totalUsersCurrent = await _context.Users.CountAsync();
            var totalUsersPrevious = await _context.Users
                .Where(u => u.CreatedAt < startCurrentPeriod)
                .CountAsync();

            var totalUsersGrowth = CalculateGrowthPercentage(totalUsersPrevious, totalUsersCurrent);
            var publishedCurrent = await _context.Notices
                .Where(n => n.PublishedDateTime >= startCurrentPeriod && n.PublishedDateTime <= now && n.Status == NoticeStatusEnum.Published)
                .CountAsync();

            var publishedPrevious = await _context.Notices
                .Where(n => n.PublishedDateTime >= startPreviousPeriod && n.PublishedDateTime <= endPreviousPeriod && n.Status == NoticeStatusEnum.Published)
                .CountAsync();

            var publishedGrowth = CalculateGrowthPercentage(publishedPrevious, publishedCurrent);

            return new AdminDashboardAnalyticsResponse
            {
                TotalUsers = totalUsersCurrent,
                TotalUsersGrowthPercent = totalUsersGrowth,

                PublishedLast30Days = publishedCurrent,
                PublishedGrowthPercent = publishedGrowth,
            };
        }

        /// <summary>
        /// To get the analytics - for admin
        /// </summary>
        /// <returns></returns>
        public async Task<AdminAnalyticsResponse> GetDetailedAnalyticsAsync()
        {
            var noticeCountsByMinistry = await _context.Notices
                .Where(n => n.Status == NoticeStatusEnum.Published)
                .GroupBy(n => n.User.Ministry)
                .Select(g => new MinistryNoticeCount
                {
                    Ministry = g.Key,
                    NoticeCount = g.Count()
                })
                .ToListAsync();
            
            var averagePublishTimesByMinistry = await _context.Notices
                .Where(n => n.Status == NoticeStatusEnum.Published && n.PublishedDateTime != default)
                .GroupBy(n => n.User.Ministry)
                .Select(g => new MinistryAveragePublishTime
                {
                    Ministry = g.Key,
                    AverageDaysToPublish = Math.Round(
                        g.Average(n => EF.Functions.DateDiffDay(n.CreatedAt, n.PublishedDateTime)), 1)
                })
                .ToListAsync();

            return new AdminAnalyticsResponse
            {
                NoticeCountsByMinistry = noticeCountsByMinistry,
                AveragePublishTimesByMinistry = averagePublishTimesByMinistry
            };
        }
        
        /// <summary>
        /// Calculate growth percentage from previous to current count
        /// </summary>
        private decimal CalculateGrowthPercentage(int previous, int current)
        {
            if (previous == 0 && current > 0)
                return 100m;
            if (previous == 0 && current == 0)
                return 0m;

            return Math.Round(((decimal)(current - previous) / previous) * 100, 2);
        }
    }