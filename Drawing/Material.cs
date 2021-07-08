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
		public static Material AlphaBlendedSprite(SpriteSheet spriteSheet) => new Material(DefaultGraphics.AlphaTestTexture, null, spriteSheet);
		public static Material AdditiveSprite(SpriteSheet spriteSheet) => new Material(DefaultGraphics.AlphaTestTexture, BlendState.Additive, spriteSheet);
		public static Material FromSprite(SpriteSheet spriteSheet) => new Material(DefaultGraphics.DefaultTexture, null, spriteSheet);
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

		public BlendState BlendState { get; private set; }

		public Effect Effect { get; private set; }
		public EffectParameterCollection EffectParameters => this.Effect.Parameters;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="blendState">If null, defaults to BlendState.AlphaBlend</param>
		/// <param name="spriteSheet"></param>
		public Material(Effect effect, BlendState blendState = null, SpriteSheet spriteSheet = null)
		{
			this.Effect = effect.Clone();
			this.BlendState = blendState ?? BlendState.AlphaBlend;
			this.SpriteSheet = spriteSheet;
		}

		public long DefaultSortingHash(float z)
		{
			return ((long)z << 32) + this.GetHashCode();
		}
	}
}
