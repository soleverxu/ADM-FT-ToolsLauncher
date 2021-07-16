using ReportConverter.Sqlite.DB.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter.Sqlite.DB
{
    static class Utils
    {
        public static object GetDBValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }

        public static bool SetDataObjectAutoIncrementValue(object dataObject, long value)
        {
            if (dataObject == null || value <= 0)
            {
                return false;
            }

            // the data object shall tag the TableAttribute
            Type t = dataObject.GetType();
            var tableAttr = t.GetCustomAttribute<TableAttribute>();
            if (tableAttr == null)
            {
                return false;
            }

            // query on each property to get the tagged primary key autoincrement constraint
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                var tableColumnAttr = pi.GetCustomAttribute<TableColumnAttribute>();
                if (tableColumnAttr == null)
                {
                    continue;
                }

                var colConstraintAttr = pi.GetCustomAttribute<TableColumnConstraintAttribute>();
                if (colConstraintAttr != null && colConstraintAttr.PrimaryKeyConstraint && colConstraintAttr.PrimaryKeyAutoIncrement)
                {
                    pi.SetValue(dataObject, value);
                    return true;
                }
            }

            return false;
        }

        public static TimeSpan? ParseTimeZoneString(string timeZone)
        {
            if (string.IsNullOrWhiteSpace(timeZone) || timeZone.Length < 9)
            {
                return null;
            }

            bool negative = timeZone[0] == '-';

            int hours = 0, minutes = 0, seconds = 0;
            string[] comps = timeZone.Substring(1).Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (comps != null)
            {
                if (comps.Length > 0)
                {
                    _ = int.TryParse(comps[0], out hours);
                }
                if (comps.Length > 1)
                {
                    _ = int.TryParse(comps[1], out minutes);
                }
                if (comps.Length > 2)
                {
                    _ = int.TryParse(comps[2], out seconds);
                }
            }

            if (negative)
            {
                hours = -1 * hours;
            }

            return new TimeSpan(hours, minutes, seconds);
        }

        public static DateTimeOffset GetDateTimeOffsetWithTimeZone(DateTime dt, string timeZone)
        {
            TimeSpan? offset = ParseTimeZoneString(timeZone);
            if (offset.HasValue)
            {
                return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, offset.Value);
            }

            return new DateTimeOffset(dt);
        }

        public static string GetISO8601DateTimeString(DateTime dt, string timeZone)
        {
            DateTimeOffset dtOffset = GetDateTimeOffsetWithTimeZone(dt, timeZone);
            return dtOffset.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        }
    }
}
