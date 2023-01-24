using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen
{
	public struct UVRect
	{
		public static UVRect One { get; private set; } = new UVRect(0, 0, 1, 1);

		public Vector2 BottomLeft => new Vector2(X, Y + Height);
		public Vector2 BottomRight => new Vector2(X + Width, Y + Height);
		public float Height { get; set; }
		public Vector2 TopLeft => new Vector2(X, Y);
		public Vector2 TopRight => new Vector2(X + Width, Y);
		public float Width { get; set; }
		public float X { get; set; }

		public float Y { get; set; }

		public UVRect(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public Rectangle FromTexture(Texture2D texture)
		{
			return new Rectangle((int)(texture.Width * X), (int)(texture.Height * Y), (int)(texture.Width * Width), (int)(texture.Height * Height));
		}
	}
}
