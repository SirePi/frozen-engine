using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.Utilities
{
	public sealed class Pool<T> where T : class, new()
	{
		private readonly Queue<T> pool = new Queue<T>();

		public T GetOne()
		{
			if (!this.pool.TryDequeue(out T result))
				result = new T();

			if (result is IPoolable poolable)
				poolable.OnPickup();

			return result;
		}

		public void ReturnOne(T obj)
		{
			if (obj == null) return;

			if (obj is IPoolable poolable)
				poolable.OnReturn();

			this.pool.Enqueue(obj);
		}
	}
}
