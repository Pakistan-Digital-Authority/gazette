namespace gop.Responses.AdminResponses;

/// <summary>
/// For admin analytics response
/// </summary>
public class AdminAnalyticsResponse
{
    /// <summary>
    /// list of notice summary
    /// </summary>
    public List<MinistryNoticeCount> NoticeCountsByMinistry { get; set; }
    /// <summary>
    /// List of average publish time
    /// </summary>
    public List<MinistryAveragePublishTime> AveragePublishTimesByMinistry { get; set; }
}

/// <summary>
/// Admin dashboard analytics response
/// </summary>
public class AdminDashboardAnalyticsResponse
{
    /// <summary>
    /// Counter of users
    /// </summary>
    public int TotalUsers { get; set; }
    /// <summary>
    /// growth percentage
    /// </summary>
    public decimal TotalUsersGrowthPercent { get; set; }
    /// <summary>
    /// publisher in last 39 days
    /// </summary>
    public int PublishedLast30Days { get; set; }
    /// <summary>
    /// pubished growth percentage
    /// </summary>
    public decimal PublishedGrowthPercent { get; set; }
    // public int ActiveSubscriptions { get; set; } // as discussed this will be changes to something else
    // public decimal ActiveSubscriptionsGrowthPercent { get; set; }
}

/// <summary>
/// Ministry notice count - analytics
/// </summary>
public class MinistryNoticeCount
{
    /// <summary>
    /// ministry - count
    /// </summary>
    public string Ministry { get; set; }
    /// <summary>
    /// Notice count
    /// </summary>
    public int NoticeCount { get; set; }
}

/// <summary>
/// Ministry average publish time
/// </summary>
public class MinistryAveragePublishTime
{
    /// <summary>
    /// ministry - average counter
    /// </summary>
    public string Ministry { get; set; }
    /// <summary>
    /// average days ot publish a notice
    /// </summary>
    public double AverageDaysToPublish { get; set; }
}