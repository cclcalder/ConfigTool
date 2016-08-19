using System.Runtime.CompilerServices;

namespace System
{
	public class EventArgs<T, K> : EventArgs
	{
		public T Data1
		{
			get;
			set;
		}

		public K Data2
		{
			get;
			set;
		}

		public EventArgs(T data1, K data2)
		{
			this.Data1 = data1;
			this.Data2 = data2;
		}

		public EventArgs(T data1)
		{
			this.Data1 = data1;
		}

		public EventArgs()
		{
		}
	}
}