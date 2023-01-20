using System;

namespace Frozen
{
	[Flags]
	public enum Alignment : byte
	{
		None = 0x00,

		Top = 0x01,

		Bottom = 0x02,

		Left = 0x04,

		Right = 0x08,

		TopLeft = Top | Left,

		TopRight = Top | Right,

		BottomLeft = Bottom | Left,

		BottomRight = Bottom | Right
	}
}
