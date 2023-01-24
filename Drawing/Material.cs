using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class Material
	{
		private Sprite spriteSheet;
		public static Material FlatColor { get; private set; } = new Material(DefaultGraphics.FlatColor);

		public BlendState BlendState { get; private set; }

		public Effect Effect { get; private set; }

		public EffectParameterCollection EffectParameters => Effect.Parameters;

		public Sprite SpriteSheet
		{
			get => spriteSheet;
			set
			{
				if (spriteSheet != value)
				{
					spriteSheet = value;
					EffectParameters["Texture"].SetValue(spriteSheet.Texture);
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="blendState">If null, defaults to BlendState.AlphaBlend</param>
		/// <param name="spriteSheet"></param>
		public Material(Effect effect, BlendState blendState = null, Sprite spriteSheet = null)
		{
			Effect = effect.Clone();
			BlendState = blendState ?? BlendState.AlphaBlend;
			SpriteSheet = spriteSheet;
		}

		internal void SetShaderParameters(Matrix view, Matrix projection)
		{
			EffectParameters["WorldViewProj"].SetValue(Matrix.Identity * view * projection);
			EffectParameters["TotalTime"]?.SetValue(Time.ScaledGameSeconds);
			EffectParameters["LastFrameTime"]?.SetValue(Time.FrameSeconds);
		}

		public static Material AdditiveSprite(Sprite spriteSheet)
		{
			return new Material(DefaultGraphics.AlphaTestTexture, BlendState.Additive, spriteSheet);
		}

		public static Material AlphaBlendedSprite(Sprite spriteSheet)
		{
			return new Material(DefaultGraphics.AlphaTestTexture, null, spriteSheet);
		}

		public static Material FromSprite(Sprite spriteSheet)
		{
			return new Material(DefaultGraphics.DefaultTexture, BlendState.NonPremultiplied, spriteSheet);
		}

		public long DefaultSortingHash(float z)
		{
			return ((long)z << 32) + GetHashCode();
		}
	}
}
