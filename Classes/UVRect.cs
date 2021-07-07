using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frozen
{
	public struct UVRect
	{
		public static UVRect One { get; private set; } = new UVRect(0, 0, 1, 1);

		public float X { get; set; }
		public float Y { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }

		public Vector2 TopLeft => new Vector2(this.X, this.Y);
		public Vector2 TopRight => new Vector2(this.X + this.Width, this.Y);
		public Vector2 BottomLeft => new Vector2(this.X, this.Y + this.Height);
		public Vector2 BottomRight => new Vector2(this.X + this.Width, this.Y + this.Height);

		public UVRect(float x, float y, float width, float height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}
	}
}
