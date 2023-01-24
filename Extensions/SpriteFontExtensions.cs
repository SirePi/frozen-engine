using System;
using System.Collections.Generic;
using System.Linq;
using FontStashSharp;
using Frozen.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen
{
	public static class SpriteFontExtensions
	{
		public static SpriteFont ToXNASpriteFont(this SpriteFontBase font, params CharacterRange[] characters)
		{
			char[] chars = characters
				.SelectMany(range => Enumerable.Range(range.Start, range.End - range.Start + 1).Select(i => (char)i))
				.Concat(new[] { '?' })
				.Distinct()
				.OrderBy(c => c)
				.ToArray();

			string str = new string(chars);

			List<Rectangle> bounds = font.GetGlyphs(str, Vector2.Zero).Select(g => g.Bounds).ToList();
			Vector2 size = font.MeasureString(str);

			GraphicsDevice gd = Engine.Game.GraphicsDevice;

			RenderTarget2D rt = new RenderTarget2D(gd, (int)MathF.Ceiling(size.X), (int)MathF.Ceiling(size.Y));
			SpriteBatch sb = new SpriteBatch(gd);

			sb.GraphicsDevice.SetRenderTarget(rt);
			sb.GraphicsDevice.Clear(Color.Transparent);
			sb.Begin();
			font.DrawText(sb, str, Vector2.Zero, Color.White);
			sb.End();

			return new SpriteFont(rt, bounds, bounds.Select(r => new Rectangle(0, r.Top, r.Width, r.Height)).ToList(), str.ToList(), font.LineHeight, 0, bounds.Select(r => new Vector3(1, r.Width, 1)).ToList(), '?');
		}
	}
}
