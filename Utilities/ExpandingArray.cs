using System;

namespace Frozen.Utilities
{
	public class ExpandingArray<T>
	{
		private int _count;
		private T[] _data;

		public int Count => _count;
		public T[] Data => _data;

		public ExpandingArray(int startingCapacity = 1024)
		{
			_data = new T[startingCapacity];
		}

		private void EnsureFit(int size)
		{
			while (_count + size > _data.Length)
				Array.Resize(ref _data, _data.Length * 2);
		}

		public void AddRange(params T[] range)
		{
			EnsureFit(range.Length);

			range.CopyTo(_data, _count);
			_count += range.Length;
		}

		public void Clear()
		{
			_count = 0;
		}
	}
}
