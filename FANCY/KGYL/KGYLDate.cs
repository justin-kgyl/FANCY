using System;
using System.Globalization;

namespace KGYL.DATE
{
    public static class KGYLDate
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime FirstDayOfMonth(this DateTime dt)
        {
            int diff = dt.Day - 1;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime LastDayOfMonth(this DateTime dt)
        {
            int diff = dt.Day - 31;
            return dt.AddDays(-1 * diff).Date;
        }
        public static string ToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }

        public static string ToMonthDayHourMinute(this DateTime dateTime)
        {
            string month = dateTime.Month.ToString().Trim();
            string day = dateTime.Day.ToString().Trim();
            string hour = dateTime.Hour.ToString().Trim();
            string minute = dateTime.Minute.ToString().Trim();

            return month + "-" + day + "_" + hour + "_" + minute;
        }

        public static string ToYearMonthDayHourMinuteSecond(this DateTime dateTime)
        {
            //string yearx = dateTime.Year.ToString();
            string year = dateTime.Year.ToString().Trim();
            string month = dateTime.Month.ToString().Trim();
            string day = dateTime.Day.ToString().Trim();
            string hour = dateTime.Hour.ToString().Trim();
            string minute = dateTime.Minute.ToString().Trim();
            string second = dateTime.Second.ToString().Trim();

            return year + month + day +  hour  + minute + second;
        }
        public static string ToYearMonthDayHourMinute(this DateTime dateTime)
        {
            //string yearx = dateTime.Year.ToString();
            string year = dateTime.Year.ToString().Trim();
            string month = dateTime.Month.ToString().Trim();
            string day = dateTime.Day.ToString().Trim();
            string hour = dateTime.Hour.ToString().Trim();
            string minute = dateTime.Minute.ToString().Trim();
            //string second = dateTime.Second.ToString().Trim();

            return year + month + day + hour + minute; // + second;
        }
    }
}