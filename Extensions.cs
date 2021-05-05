using FrozenEngine.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine
{
	public static class PointExtensions
	{
		public static Vector2 ToVector2(this Point point)
		{
			return new Vector2(point.X, point.Y);
		}
	}

	public static class Vector3Extensions
	{
		public static Vector2 XY(this Vector3 vector)
		{
			return new Vector2(vector.X, vector.Y);
		}
	}

	public static class RectangleExtensions
	{
		public static Rectangle Transform2D(this Rectangle rect, Vector2 offset)
		{
			return rect.Transform2D(offset, Vector2.One);
		}

		public static Rectangle Transform2D(this Rectangle rect, Vector2 offset, float scale)
		{
			return rect.Transform2D(offset, new Vector2(scale));
		}

		public static Rectangle Transform2D(this Rectangle rect, Vector2 offset, Vector2 scale)
		{
			Rectangle result = new Rectangle(0, 0, (int)Math.Ceiling(rect.Width * scale.X), (int)Math.Ceiling(rect.Height * scale.Y));
			result.Offset(offset);
			return result;
		}

		public static bool IsInfinite(this Rectangle rect)
		{
			return rect == CoreMath.InfiniteRectangle;
		}
	}
}
