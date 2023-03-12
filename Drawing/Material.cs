using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

		public RenderTarget2D[] RenderTargets { get; private set; } = new RenderTarget2D[1];

		protected Dictionary<int, Action<GraphicsDevice>> _passSetups = new Dictionary<int, Action<GraphicsDevice>>();

		public Sprite SpriteSheet
		{
			get => spriteSheet;
			set
			{
				if (spriteSheet != value)
				{
					spriteSheet = value;
					EffectParameters["Texture"]?.SetValue(spriteSheet.Texture);
					UpdateRenderTargets();
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

		private void UpdateRenderTargets()
		{
			if (RenderTargets != null)
				for (int i = 0; i < RenderTargets.Length; i++)
					RenderTargets[i]?.Dispose();
			
			int maxPasses = Effect.Techniques.Select(t => t.Passes.Count).Max();
			RenderTargets = Enumerable.Range(0, maxPasses).Select(i => i == maxPasses - 1 ? null : new RenderTarget2D(Engine.Game.GraphicsDevice, SpriteSheet.Texture.Width, SpriteSheet.Texture.Height)).ToArray();
		}

		internal void SetDefaultEffectParameters(Matrix view, Matrix projection)
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

		private int _currentPass;
		internal void Begin(GraphicsDevice device)
		{
			_currentPass = 0;
			device.BlendState = BlendState;
		}

		public IEnumerable<EffectPass> CurrentTechniquePasses(GraphicsDevice device)
		{
			while (_currentPass < Effect.CurrentTechnique.Passes.Count)
			{
				if (_passSetups.TryGetValue(_currentPass, out Action<GraphicsDevice> setup))
					setup(device);

				yield return Effect.CurrentTechnique.Passes[_currentPass];
				_currentPass++;
			}
		}
	}
}
