using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnitySocialCloudSave.Utils
{
    public static class SdkUtils
    {
        public static readonly string UnknownIdentityId = "unknown";

        private static DateTime _epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ConvertFromUnixEpochSeconds(string milliseconds)
        {
            return new DateTime(Convert.ToInt64(milliseconds) * 10000L + _epochStart.Ticks, DateTimeKind.Utc);
        }

        public static string ConvertToUnixEpochMilliSeconds(DateTime? dateTime)
        {
            if (dateTime == null) return "";
            TimeSpan ts = new TimeSpan(dateTime.Value.ToUniversalTime().Ticks - _epochStart.Ticks);
            double milli = Math.Round(ts.TotalMilliseconds, 0);
            return milli.ToString(CultureInfo.InvariantCulture);
        }

        public static string ConvertDateTimeToTicks(DateTime dateTime)
        {
            return dateTime.Ticks.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        public static DateTime ConvertTicksToDateTime(string ticks)
        {
            return new DateTime(long.Parse(ticks, CultureInfo.InvariantCulture), DateTimeKind.Utc);
        }

        public static string ConvertSyncRevisionToString(IList<SyncRevision> revisions)
        {
            string result = "";
            if (revisions != null && revisions.Count > 0)
                for (int i = 0; i < revisions.Count; i++)
                {
                    result += revisions[i].SyncRegion + ":" + revisions[i].SyncCount + (i == revisions.Count - 1 ? "" : ",");
                }

            return result;
        }

        public static IList<SyncRevision> ConvertStringToSyncRevision(string regions)
        {
            IList<SyncRevision> result = new List<SyncRevision>();
            if (!string.IsNullOrEmpty(regions))
            {
                var split = regions.Split(',');
                foreach (var rev in split)
                {
                    var revSplit = rev.Split(':');
                    result.Add(new SyncRevision
                    {
                        SyncRegion = revSplit[0].Trim(),
                        SyncCount = long.Parse(revSplit[1].Trim())
                    });
                }
            }

            return result;
        }
    }
}