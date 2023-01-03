using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.Utilities
{
	public interface IPoolable
	{
		void OnPickup();
		void OnReturn();
	}
}
