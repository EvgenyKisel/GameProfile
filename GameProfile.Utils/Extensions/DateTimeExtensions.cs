using System.Globalization;

namespace GameProfile.Utils.Extensions;

public static class DateTimeExtensions
{
    private const string UtcDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

    public static string ToUtcString(this DateTime dateTime)
    {
        return dateTime.ToString(UtcDateTimeFormat, CultureInfo.InvariantCulture);
    }
}
