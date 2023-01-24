using Microsoft.Xna.Framework;

namespace Frozen.Extensions
{
	public static class RectangleExtensions
	{
		public static bool IsInfinite(this Rectangle rect)
		{
			return rect == FrozenMath.InfiniteRectangle;
		}

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
			Rectangle result = new Rectangle(0, 0, (int)System.Math.Ceiling(rect.Width * scale.X), (int)System.Math.Ceiling(rect.Height * scale.Y));
			result.Offset(offset);
			return result;
		}
	}
}
