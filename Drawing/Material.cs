using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.Drawing
{
	public class Material
	{
		public static Material White { get; private set; } = new Material(null, new DrawTechnique());

		public SpriteSheet SpriteSheet { get; private set; }
		public DrawTechnique Technique { get; private set; }
		internal AlphaTestEffect Effect { get; private set; }
		internal bool IsCompiled { get; private set; }

		internal void Compile(GraphicsDevice device)
		{
			this.Effect = new AlphaTestEffect(device)
			{
				Texture = this.SpriteSheet.Texture,
				VertexColorEnabled = true
			};

			this.IsCompiled = true;
		}

		public Material(SpriteSheet spriteSheet, DrawTechnique technique)
		{
			this.SpriteSheet = spriteSheet;
			this.Technique = technique;
		}

		public long DefaultSortingHash(float z)
		{
			return ((long)z << 32) + this.GetHashCode();
		}
	}
}
