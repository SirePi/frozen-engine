using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class Font
	{
		public struct GlyphInfo
		{
			public SpriteFont.Glyph Glyph { get; set; }

			public float UVBottom { get; set; }
			public float UVLeft { get; set; }
			public float UVRight { get; set; }
			public float UVTop { get; set; }
		}

		private Dictionary<char, GlyphInfo> _glyphs;

		public SpriteFont XnaFont { get; private set; }
		public Material Material { get; private set; }
		public float LineHeight { get; private set; }

		public Font(SpriteFont font)
		{
			XnaFont = font;
			LineHeight = font.MeasureString("Wq").Y;
			Material = Material.AlphaBlendedSprite(new Sprite(font.Texture));

			Vector2 texelSize = Vector2.One / font.Texture.Bounds.Size.ToVector2();
			_glyphs = font.GetGlyphs().ToDictionary(
			k => k.Key,
			v =>
			{
				Vector2 topLeft = v.Value.BoundsInTexture.Location.ToVector2() * texelSize;
				Vector2 bottomRight = (v.Value.BoundsInTexture.Location + v.Value.BoundsInTexture.Size).ToVector2() * texelSize;
				return new GlyphInfo
				{
					Glyph = v.Value,
					UVTop = topLeft.Y,
					UVBottom = bottomRight.Y,
					UVLeft = topLeft.X,
					UVRight = bottomRight.X
				};
			});
		}

		public bool TryGetGlyph(char c, out GlyphInfo glyph)
		{
			if (!_glyphs.TryGetValue(c, out glyph))
			{
				if (XnaFont.DefaultCharacter.HasValue)
					glyph = _glyphs[XnaFont.DefaultCharacter.Value];
				else
					return false;
			}

			return true;
		}
	}
}
