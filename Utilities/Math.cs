using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.Utilities
{
	public static class CoreMath
	{
		public const float TWO_PI = MathF.PI * 2;
		public static Rectangle InfiniteRectangle { get; private set; } = new Rectangle(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);

		private static bool[] multiplesOfThree = Enumerable
			.Range(0, 1024000)
			.Select(i => i % 3 == 0)
			.ToArray();

		public static bool IsMultipleOf2(int value)
		{
			return (value & 0x01) == 0;
		}

		public static bool IsMultipleOf3(int value)
		{
			return multiplesOfThree[value];
		}

		public static float Min(params float[] values)
		{
			return values.Min();
		}

		public static float Max(params float[] values)
		{
			return values.Max();
		}
	}
}
