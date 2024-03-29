﻿using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public static class DefaultGraphics
	{
		public static AlphaTestEffect AlphaTestTexture { get; private set; } = new AlphaTestEffect(Engine.Game.GraphicsDevice) { VertexColorEnabled = true };

		public static AlphaTestEffect DefaultTexture { get; private set; } = new AlphaTestEffect(Engine.Game.GraphicsDevice) { VertexColorEnabled = true, FogEnabled = false };

		public static BasicEffect FlatColor { get; private set; } = new BasicEffect(Engine.Game.GraphicsDevice) { TextureEnabled = false, VertexColorEnabled = true };
	}
}
