namespace gop.Extensions;

/// <summary>
/// To get the human-readable date time.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// To get the relative time
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var ts = DateTime.UtcNow - dateTime.ToUniversalTime();
        
        if (ts.TotalSeconds < 60)
            return "Updated just now";

        if (ts.TotalMinutes < 60)
            return $"Updated {Math.Floor(ts.TotalMinutes)} minute{(ts.TotalMinutes >= 2 ? "s" : "")} ago";

        if (ts.TotalHours < 24)
            return $"Updated {Math.Floor(ts.TotalHours)} hour{(ts.TotalHours >= 2 ? "s" : "")} ago";

        if (ts.TotalDays < 7)
            return $"Updated {Math.Floor(ts.TotalDays)} day{(ts.TotalDays >= 2 ? "s" : "")} ago";

        if (ts.TotalDays < 30)
        {
            int weeks = (int)Math.Floor(ts.TotalDays / 7);
            return $"Updated {weeks} week{(weeks >= 2 ? "s" : "")} ago";
        }

        if (ts.TotalDays < 365)
        {
            int months = (int)Math.Floor(ts.TotalDays / 30);
            return $"Updated {months} month{(months >= 2 ? "s" : "")} ago";
        }

        int years = (int)Math.Floor(ts.TotalDays / 365);
        return $"Updated {years} year{(years >= 2 ? "s" : "")} ago";
    }
}
