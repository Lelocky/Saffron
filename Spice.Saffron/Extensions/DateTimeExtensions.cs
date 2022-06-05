using System.Globalization;

namespace System
{
    public static class DateTimeExtensions
    {
        public static DateTime ClearDate(this DateTime dt)
        {
            var clearedDate =  dt.AddYears(1972 - dt.Year); //1972 is a leap year

            return DateTime.SpecifyKind(clearedDate, DateTimeKind.Utc);
        }

        public static bool IsLeapYear(this DateTime source)
        {
            return DateTime.IsLeapYear(source.Year);
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        //https://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date
        public static int GetIso8601WeekOfYear(this DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime ChangeToCurrentYear(this DateTime time)
        {
            return time.ChangeYear(DateTime.Now.Year);
        }


        public static DateTime ChangeYear(this DateTime dt, int newYear)
        {
            return dt.AddYears(newYear - dt.Year);
        }
    }
}
