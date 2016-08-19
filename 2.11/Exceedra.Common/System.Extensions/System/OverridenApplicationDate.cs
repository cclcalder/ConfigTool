using System.Runtime.CompilerServices;

namespace System
{
	internal class OverridenApplicationDate
	{
		public DateTime Now
		{
			get;
			internal set;
		}

		public OverridenApplicationDate(DateTime time)
		{
			this.Now = time;
		}
	}
}