using System.Runtime.CompilerServices;

namespace System
{
	public class EventArgs<T> : EventArgs
	{
		public T Data
		{
			get;
			set;
		}

		public EventArgs(T data)
		{
			this.Data = data;
		}

		public EventArgs()
		{
		}
	}
}