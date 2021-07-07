using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.Drawing
{
	public class Material
	{
		public static Material FlatColor { get; private set; } = new Material(DefaultGraphics.FlatColor);

#pragma warning disable IDE0022 // Use block body for methods - suppressed for clarity
		public static Material AlphaBlendedSprite(SpriteSheet spriteSheet) => new Material(DefaultGraphics.AlphaTestTexture, spriteSheet);
		public static Material FromSprite(SpriteSheet spriteSheet) => new Material(DefaultGraphics.DefaultTexture, spriteSheet);
#pragma warning restore IDE0022 // Use block body for methods

		private SpriteSheet spriteSheet;
		public SpriteSheet SpriteSheet
		{
			get => this.spriteSheet;
			set
			{
				if (this.spriteSheet != value)
				{
					this.spriteSheet = value;
					this.EffectParameters["Texture"].SetValue(this.spriteSheet.Texture);
				}
			}
		}

		public Effect Effect { get; private set; }
		public EffectParameterCollection EffectParameters => this.Effect.Parameters;

		public Material(Effect effect, SpriteSheet spriteSheet = null)
		{
			this.Effect = effect.Clone();
			this.SpriteSheet = spriteSheet;
		}

		public long DefaultSortingHash(float z)
		{
			return ((long)z << 32) + this.GetHashCode();
		}
	}
}
