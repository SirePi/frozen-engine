using System;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace Frozen
{
	[Flags]
	public enum Alignment : byte
	{
		Center = 0x00,

		Top = 0x01,

		Bottom = 0x02,

		Left = 0x04,

		Right = 0x08,

		TopLeft = Top | Left,

		TopRight = Top | Right,

		BottomLeft = Bottom | Left,

		BottomRight = Bottom | Right,
	}

	internal static class AlignmentExtensions
	{
		public static TextHorizontalAlignment ToFontStashSharpAlignment(this Alignment value)
		{
			if (value.HasFlag(Alignment.Left))
				return TextHorizontalAlignment.Left;
			else if (value.HasFlag(Alignment.Right))
				return TextHorizontalAlignment.Right;
			else
				return TextHorizontalAlignment.Center;
		}

		public static Vector2 ToFontStashSharpOrigin(this Alignment value, Point textSize)
		{
			if (value.HasFlag(Alignment.Left))
				return Vector2.Zero;
			else if (value.HasFlag(Alignment.Right))
				return new Vector2(textSize.X, 0);
			else
				return new Vector2(textSize.X / 2, 0);
		}
	}
}
