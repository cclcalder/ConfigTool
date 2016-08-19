namespace System
{
	public class LocalTime
	{
		public static DateTime Now
		{
			get
			{
				DateTime dateTime;
				OverridenApplicationDate current = ProcessContext<OverridenApplicationDate>.Current;
				dateTime = (current == null ? DateTime.Now : current.Now);
				return dateTime;
			}
		}

		public static DateTime Today
		{
			get
			{
				return LocalTime.Now.Date;
			}
		}

		public LocalTime()
		{
		}

		public static void Add(TimeSpan time)
		{
			OverridenApplicationDate current = ProcessContext<OverridenApplicationDate>.Current;
			if (current == null)
			{
				throw new InvalidOperationException("The current thread is not running inside a LocalTime.");
			}
			current.Now = current.Now.Add(time);
		}

		public static void AddDays(double days)
		{
			LocalTime.Add(TimeSpan.FromDays(days));
		}

		public static void AddHours(double hours)
		{
			LocalTime.Add(TimeSpan.FromHours(hours));
		}

		public static void AddMinutes(double minutes)
		{
			LocalTime.Add(TimeSpan.FromMinutes(minutes));
		}

		public static void AddSeconds(double seconds)
		{
			LocalTime.Add(TimeSpan.FromSeconds(seconds));
		}

		public static IDisposable Set(DateTime overridenNow)
		{
			return new ProcessContext<OverridenApplicationDate>(new OverridenApplicationDate(overridenNow));
		}

		public static IDisposable Stop()
		{
			return LocalTime.Set(DateTime.Now);
		}
	}
}