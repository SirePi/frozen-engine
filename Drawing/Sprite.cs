using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class Sprite
	{
		private Texture2D texture;

		public Texture2D Texture
		{
			get => this.texture;
			set => this.texture = value ?? throw new ArgumentNullException(nameof(this.Texture));
		}

		public Atlas Atlas { get; private set; }

		public Sprite(Texture2D texture)
		{
			this.Texture = texture;
			this.Atlas = Atlas.SingleSprite();
		}

		public Sprite(Texture2D texture, int rows, int columns)
		{
			this.Texture = texture;
			this.Atlas = Atlas.FromGrid(rows, columns);
		}

		public Rectangle this[int spriteIndex]
		{
			get => this.GetRealRectangle(this.Atlas[spriteIndex]);
		}

		public Rectangle this[string spriteName]
		{
			get => this.GetRealRectangle(this.Atlas[spriteName]);
		}

		private Rectangle GetRealRectangle(UVRect uv)
		{
			return new Rectangle((int)(this.texture.Width * uv.X), (int)(this.texture.Height * uv.Y), (int)(this.texture.Width * uv.Width), (int)(this.texture.Height * uv.Height));
		}
	}
}
