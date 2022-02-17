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
    }
}
