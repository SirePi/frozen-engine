using System;
using System.Collections.Generic;

namespace Frozen.Utilities
{
	public sealed class DerivedObjectsPool<T> where T : class
	{
		private readonly List<T> _pool = new List<T>();

		private int _poolIndex = 0;

		public V GetOne<V>(Func<V> builder) where V : class, T
		{
			int searchIndex;
			V result = null;

			for (int i = _poolIndex; i < _pool.Count + _poolIndex; i++)
			{
				searchIndex = i % _pool.Count;

				result = _pool[searchIndex] as V;

				if (result != null)
				{
					_poolIndex = searchIndex;
					_pool.RemoveAt(_poolIndex);

					break;
				}
			}

			if (result == null)
				result = builder();

			if (result is IPoolable poolable)
				poolable.OnPickup();

			return result;
		}

		public void ReturnOne(T obj)
		{
			if (obj is IPoolable poolable)
				poolable.OnReturn();

			if (obj != null)
				_pool.Add(obj);
		}
	}
}
