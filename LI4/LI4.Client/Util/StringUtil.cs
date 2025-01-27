namespace LI4.Client.Util;
using System.Globalization;

public class StringUtil {
    public static string reverseInterpolateBlockName(string blockName) {
        TextInfo textInfo = new CultureInfo("pt-PT", false).TextInfo;

        string name = string.Join("_", blockName.Split(" ").Select(x => textInfo.ToLower(x)).ToArray());
        return name;
    }

    public static string formatTimeSpan(long time) {
        TimeSpan ts = TimeSpan.FromSeconds(time);

        string tss = "";
        if (ts.Days != 0) tss += ("" + Math.Abs(ts.Days)).PadLeft(2, '0') + "d";
        tss += ("" + Math.Abs(ts.Hours)).PadLeft(2, '0') + "h" + ("" + Math.Abs(ts.Minutes)).PadLeft(2, '0') + "m" + ("" + Math.Abs(ts.Seconds)).PadLeft(2, '0') + "s";

        return tss;
    }

    public static string formatTime(DateTime time) {
        return time.ToString("dd/MM/yyyy HH:mm:ss");
    }

    public static DateTime convertFromUnixTimestamp(long timestamp) {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return origin.AddSeconds(timestamp);
    }

    public static long convertToUnixTimestamp(DateTime date) {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;

        return (long)Math.Floor(diff.TotalSeconds);
    }
}
