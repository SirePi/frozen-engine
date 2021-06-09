using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FrozenEngine.Drawing
{
	public static class DefaultGraphics
	{
		public static AlphaTestEffect AlphaTestTexture { get; private set; } = new AlphaTestEffect(Frozen.Game.GraphicsDevice) { VertexColorEnabled = true };
		public static BasicEffect DefaultTexture { get; private set; } = new BasicEffect(Frozen.Game.GraphicsDevice) { TextureEnabled = true, VertexColorEnabled = true };
		public static BasicEffect FlatColor { get; private set; } = new BasicEffect(Frozen.Game.GraphicsDevice) { TextureEnabled = false, VertexColorEnabled = true };
	}
}
