using GeoTimeZone;

namespace gop.Utilities;

public static  class GeoToTimezoneConverter
{
    public static string GetTimeZone(double latitude, double longitude)
    {
        try
        {
            var tz = TimeZoneLookup.GetTimeZone(latitude, longitude);
            return tz.Result;
        }
        catch
        {
            return "UTC";
        }
    }
}