using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class Material
	{
		public static Material FlatColor { get; private set; } = new Material(DefaultGraphics.FlatColor);

#pragma warning disable IDE0022 // Use block body for methods - suppressed for clarity

		public static Material AlphaBlendedSprite(Sprite spriteSheet) => new Material(DefaultGraphics.AlphaTestTexture, null, spriteSheet);

		public static Material AdditiveSprite(Sprite spriteSheet) => new Material(DefaultGraphics.AlphaTestTexture, BlendState.Additive, spriteSheet);

		public static Material FromSprite(Sprite spriteSheet) => new Material(DefaultGraphics.DefaultTexture, BlendState.NonPremultiplied, spriteSheet);

#pragma warning restore IDE0022 // Use block body for methods

		private Sprite spriteSheet;

		public Sprite SpriteSheet
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
		public Material(Effect effect, BlendState blendState = null, Sprite spriteSheet = null)
		{
			this.Effect = effect.Clone();
			this.BlendState = blendState ?? BlendState.AlphaBlend;
			this.SpriteSheet = spriteSheet;
		}

		internal void SetShaderParameters(Matrix view, Matrix projection)
		{
			this.EffectParameters["WorldViewProj"].SetValue(Matrix.Identity * view * projection);
			this.EffectParameters["TotalTime"]?.SetValue(Time.TotalGameSeconds);
			this.EffectParameters["LastFrameTime"]?.SetValue(Time.FrameSeconds);
		}

		public long DefaultSortingHash(float z)
		{
			return ((long)z << 32) + this.GetHashCode();
		}
	}
}
