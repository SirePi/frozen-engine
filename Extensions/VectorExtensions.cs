using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frozen.Extensions
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
	}
}
