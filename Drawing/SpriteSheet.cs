using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class SpriteSheet
	{
		private Texture2D texture;

		public Texture2D Texture 
		{
			get => this.texture;
			set => this.texture = value ?? throw new InvalidOperationException("");
		}
		public SpriteAtlas Atlas { get; private set; }

		public SpriteSheet(Texture2D texture)
		{
			this.Texture = texture;
			this.Atlas = SpriteAtlas.SingleSprite();
		}

		public SpriteSheet(Texture2D texture, int rows, int columns)
		{
			this.Texture = texture;
			this.Atlas = SpriteAtlas.FromGrid(rows, columns);
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
