using System;

namespace EOHax.Threading
{
	public class Future<T>
	{
		public delegate R FutureDelegate<R>();
		private FutureDelegate<T> future_delegate;
		private IAsyncResult result;
		private T value;
		private bool has_value = false;

		public T Value
		{
			get
			{
				if (has_value)
					return value;

				if (!result.IsCompleted)
					result.AsyncWaitHandle.WaitOne();

				value = future_delegate.EndInvoke(result);
				has_value = true;

				return value;
			}
		}

		public Future(FutureDelegate<T> future_delegate)
		{
			this.future_delegate = future_delegate;
			result = future_delegate.BeginInvoke(null, null);
		}
		
		public bool Finished()
		{
			return result.IsCompleted;
		}

		public static implicit operator T(Future<T> f)
		{
			return f.Value;
		}
	}
}
