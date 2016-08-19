using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace System
{
	public class ProcessContext<T> : IDisposable
	where T : class
	{
		private static object staticLock;

		private static Func<T> DefaultDataExpression;

		private static List<ProcessContext<T>> Contexts
		{
			get
			{
				List<ProcessContext<T>> data;
				lock (ProcessContext<T>.staticLock)
				{
					if (CallContext.GetData(ProcessContext<T>.ProcessContextKey) != null)
					{
						data = CallContext.GetData(ProcessContext<T>.ProcessContextKey) as List<ProcessContext<T>>;
					}
					else
					{
						List<ProcessContext<T>> processContexts = new List<ProcessContext<T>>();
						CallContext.SetData(ProcessContext<T>.ProcessContextKey, processContexts);
						data = processContexts;
					}
				}
				return data;
			}
		}

		public static T Current
		{
			get
			{
				T t;
				t = (ProcessContext<T>.Contexts.Count != 0 ? ProcessContext<T>.Contexts.Last<ProcessContext<T>>().Data : ProcessContext<T>.DefaultDataExpression());
				return t;
			}
		}

		public T Data
		{
			get;
			private set;
		}

		private static string ProcessContextKey
		{
			get
			{
				return string.Concat("ProcessContext:", typeof(T).FullName);
			}
		}

		static ProcessContext()
		{
			ProcessContext<T>.staticLock = new object();
			ProcessContext<T>.DefaultDataExpression = () => default(T);
		}

		public ProcessContext(T data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this.Data = data;
			ProcessContext<T>.Contexts.Add(this);
		}

		public void Dispose()
		{
			try
			{
				ProcessContext<T>.Contexts.Remove(this);
			}
			catch
			{
			}
		}

		public static void SetDefaultDataExpression(Func<T> expression)
		{
			if (expression == null)
			{
				expression = () => default(T);
			}
			ProcessContext<T>.DefaultDataExpression = expression;
		}
	}
}