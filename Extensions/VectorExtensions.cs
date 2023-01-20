using System;
using Microsoft.Xna.Framework;

namespace Frozen
{
	public static class Vector2Extensions
	{
		public static float GetAngle(this Vector2 vector)
		{
			return MathF.Atan2(vector.Y, vector.X);
		}
	}

	public static class Vector3Extensions
	{
		public static Vector2 XY(this Vector3 vector)
		{
			return new Vector2(vector.X, vector.Y);
		}

		public static Vector3 RoundToPixel(this Vector3 vector)
		{
			vector.X = RoundFloat(vector.X);
			vector.Y = RoundFloat(vector.Y);

			return vector;
		}

		private static float RoundFloat(float f)
		{
			return f % 1 > .5f ? MathF.Ceiling(f) : MathF.Floor(f);
		}
	}
}
