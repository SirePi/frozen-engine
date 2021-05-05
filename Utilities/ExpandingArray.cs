using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.Utilities
{
	public class ExpandingArray<T>
	{
		private T[] data;
		private int count;

		public T[] Data => this.data;
		public int Count => this.count;

		public ExpandingArray(int startingCapacity = 1024)
		{
			this.data = new T[startingCapacity];
		}

		public void Clear()
		{
			this.count = 0;
		}

		public void AddRange(params T[] range)
		{
			this.EnsureFit(range.Length);

			range.CopyTo(this.data, this.count);
			this.count += range.Length;
		}

		private void EnsureFit(int size)
		{
			while (this.count + size > this.data.Length)
				Array.Resize(ref this.data, this.data.Length * 2);
		}
	}
}
