using System;
using System.Collections.Generic;

namespace Frozen.Utilities
{
	public sealed class Pool<T> where T : class
	{
		private readonly Func<T> _factory;

		private readonly Queue<T> _pool = new Queue<T>();

		public Pool(Func<T> factory)
		{
			_factory = factory;
		}

		public T GetOne()
		{
			if (!_pool.TryDequeue(out T result))
				result = _factory();

			if (result is IPoolable poolable)
				poolable.OnPickup();

			return result;
		}

		public void ReturnOne(T obj)
		{
			if (obj == null)
				return;

			if (obj is IPoolable poolable)
				poolable.OnReturn();

			_pool.Enqueue(obj);
		}
	}
}
