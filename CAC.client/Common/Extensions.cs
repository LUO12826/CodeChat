using Microsoft.Toolkit.Uwp.Helpers;
using System;

namespace CAC.client.Common
{
    /// <summary>
    /// 对系统类的拓展。
    /// </summary>
    public static class Extensions
    {
        
        public static bool IsNullOrEmpty(this string a)
        {
            return a == null || a == "";
        }

        /// <summary>
        /// 判断这个日期和另一个日期指示的是否是同一天。
        /// </summary>
        public static bool IsSameDay(this DateTime a, DateTime b)
        {
            return a.Year == b.Year
                && a.Month == b.Month
                && a.Day == b.Day;
        }

        /// <summary>
        /// 判断这个日期和另一个日期是否在同一周。默认以周一为一周的开始。
        /// 由于不知道DateTime类的每周起始时间会不会因地区而发生变化，这个方法可能有bug
        /// </summary>
        public static bool IsSameWeek(this DateTime a, DateTime b, bool isWeekStartAtSun = false)
        {
            int offset = 0;
            if (isWeekStartAtSun) {
                offset = -1;
            }

            int aDayOfWeek = (int)a.DayOfWeek;
            int aDays = (int)a.TimeSpanForm1970().TotalDays + offset;
            int bDays = (int)b.TimeSpanForm1970().TotalDays;

            if (bDays >= aDays + 1 - aDayOfWeek && bDays <= aDays + 7 - aDayOfWeek)
                return true;
            return false;
        }
        
        /// <summary>
        /// 获取一个日期自1970年1月1日(UTC)起的时间间隔。
        /// </summary>
        public static TimeSpan TimeSpanForm1970(this DateTime a)
        {
            return a.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
        }


        /// <summary>
        /// 将时间简单转化为便于阅读的时间
        /// </summary>
        public static string ToConvenientTimeString(this DateTime a)
        {
            int hour = a.Hour;
            string timeString = a.ToString("h:mm");
            if(hour < 5) {
                return "凌晨" + timeString;
            }
            else if(hour < 12) {
                return "上午" + timeString;
            }
            else if(hour < 14) {
                return "中午" + timeString;
            }
            else if(hour < 19) {
                return "下午" + timeString;
            }
            else if(hour < 24) {
                return "晚上" + timeString;
            }
            return "";
        }

        public static string ToChineseString(this DayOfWeek a)
        {
            switch (a) {
                case DayOfWeek.Monday:
                    return "星期一";
                case DayOfWeek.Tuesday:
                    return "星期二";
                case DayOfWeek.Wednesday:
                    return "星期三";
                case DayOfWeek.Thursday:
                    return "星期四";
                case DayOfWeek.Friday:
                    return "星期五";
                case DayOfWeek.Saturday:
                    return "星期六";
                case DayOfWeek.Sunday:
                    return "星期日";
                default:
                    return "";
            }
        }
        
        /// <summary>
        /// 将时间转换为直白的时间字符串。
        /// </summary>
        public static string ToExplicitString(this DateTime dateTime)
        {
            var nowTime = DateTime.Now;

            //如果这个时间晚于当前的时间。目前没有仔细考虑这样的情况。
            if (dateTime >= nowTime) {
                return dateTime.ToString("yyyy/M/d");
            }
            //如果时间和当前时间是同一天
            if (dateTime.IsSameDay(nowTime)) {
                return dateTime.ToConvenientTimeString();
            }
            //如果是昨天
            else if (dateTime.IsSameDay(nowTime.AddDays(-1))) {
                return "昨天";
            }
            //如果是前天
            else if (dateTime.IsSameDay(nowTime.AddDays(-2))) {
                return "前天";
            }
            //如果是同一周
            else if (dateTime.IsSameWeek(nowTime)) {
                return dateTime.DayOfWeek.ToChineseString();
            }
            //如果是同一年
            else if (dateTime.Year == nowTime.Year) {
                return dateTime.ToString("M/d");
            }
            return dateTime.ToString("yyyy/M/d");
        }

        /// <summary>
        /// 将时间转换为直白的时间字符串。这个版本总是会显示精确到分钟的时间
        /// </summary>
        public static string ToExplicitStringVer2(this DateTime dateTime)
        {
            var nowTime = DateTime.Now;

            //如果这个时间晚于当前的时间。目前没有仔细考虑这样的情况。
            if (dateTime >= nowTime) {
                return dateTime.ToString("yyyy/M/d H:mm");
            }
            //如果时间和当前时间是同一天
            if (dateTime.IsSameDay(nowTime)) {
                return dateTime.ToConvenientTimeString();
            }
            //如果是昨天
            else if (dateTime.IsSameDay(nowTime.AddDays(-1))) {
                return "昨天" + dateTime.ToString("H:mm");
            }
            //如果是前天
            else if (dateTime.IsSameDay(nowTime.AddDays(-2))) {
                return "前天" + dateTime.ToString("H:mm");
            }
            //如果是同一年
            else if (dateTime.Year == nowTime.Year) {
                return dateTime.ToString("M/d") + dateTime.ToString("H:mm");
            }
            return dateTime.ToString("yyyy/M/d H:mm");
        }
    }
}
