using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.Utilities
{
	public sealed class Pool<T> where T : class
	{
		private readonly Func<T> factory;
		private readonly Queue<T> pool = new Queue<T>();

		public Pool(Func<T> factory)
		{
			this.factory = factory;
		}

		public T GetOne()
		{
			if (!this.pool.TryDequeue(out T result))
				result = this.factory();

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
