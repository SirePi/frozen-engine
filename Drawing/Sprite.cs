using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class Sprite
	{
		private Texture2D _texture;

		public Atlas Atlas { get; private set; }

		public Texture2D Texture
		{
			get => _texture;
			set => _texture = value ?? throw new ArgumentNullException(nameof(Texture));
		}

		public Sprite(Texture2D texture)
		{
			Texture = texture;
			Atlas = Atlas.SingleSprite();
		}

		public Sprite(Texture2D texture, int rows, int columns)
		{
			Texture = texture;
			Atlas = Atlas.FromGrid(rows, columns);
		}

		public Sprite(Texture2D texture, params Rectangle[] sprites)
		{
			Texture = texture;

			Vector2 uv = Vector2.One / Texture.Bounds.Size.ToVector2();

			Atlas = Atlas.FromRects(sprites.Select(rect =>
			{
				Vector2 location = rect.Location.ToVector2() * uv;
				Vector2 size = rect.Size.ToVector2() * uv;
				return new UVRect(location, size);
			}));
		}

		public Rectangle this[int spriteIndex]
		{
			get => Atlas[spriteIndex].FromTexture(_texture);
		}

		public Rectangle this[string spriteName]
		{
			get => Atlas[spriteName].FromTexture(_texture);
		}
	}
}
