using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Frozen
{
	public static class FrozenMath
	{
		private static bool[] multiplesOfThree = Enumerable
			.Range(0, 1024000)
			.Select(i => i % 3 == 0)
			.ToArray();

		public const float TWO_PI = MathF.PI * 2;
		public static Rectangle InfiniteRectangle { get; private set; } = new Rectangle(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);

		public static bool IsBetween(float value, float a, float b)
		{
			if (a < b)
				return a <= value && value <= b;
			else
				return b <= value && value <= a;
		}

		public static bool IsMultipleOf2(int value)
		{
			return (value & 0x01) == 0;
		}

		public static bool IsMultipleOf3(int value)
		{
			return multiplesOfThree[value];
		}

		public static float Max(params float[] values)
		{
			return values.Max();
		}

		public static float Min(params float[] values)
		{
			return values.Min();
		}
	}
}
