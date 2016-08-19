using System.Linq;
using System.Runtime.CompilerServices;

namespace System
{
	public static class DateTimeExtensions
	{
		public static TimeSpan Days(this int number)
		{
			return TimeSpan.FromDays((double)number);
		}

		public static TimeSpan Hours(this int number)
		{
			return TimeSpan.FromHours((double)number);
		}

		public static bool IsHoliday(this DateTime date)
		{
			DayOfWeek[] dayOfWeekArray = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
			return dayOfWeekArray.Contains<DayOfWeek>(date.DayOfWeek);
		}

		public static bool IsNewerThan(this DateTime source, TimeSpan span)
		{
			return (LocalTime.Now - source) < span;
		}

		public static bool IsOlderThan(this DateTime source, TimeSpan span)
		{
			return (LocalTime.Now - source) > span;
		}

		public static TimeSpan Milliseconds(this int number)
		{
			return TimeSpan.FromMilliseconds((double)number);
		}

		public static TimeSpan Minutes(this int number)
		{
			return TimeSpan.FromMinutes((double)number);
		}

		public static TimeSpan Seconds(this int number)
		{
			return TimeSpan.FromSeconds((double)number);
		}

		public static TimeSpan Ticks(this int number)
		{
			return TimeSpan.FromTicks((long)number);
		}
	}
}