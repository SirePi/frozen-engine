using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine
{
	[Flags]
	public enum InputState : byte
	{
		Up = 0x01,
		Hit = 0x02,
		Held = 0x04,
		Down = Hit | Held
	}
}
