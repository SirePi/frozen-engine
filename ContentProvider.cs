using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Frozen
{
	public class ContentProvider
	{
		private Dictionary<string, object> cache = new Dictionary<string, object>();

		private bool audioEnabled = true;
		private Game game;
		internal DefaultContent DefaultContent { get; private set; }

		internal ContentProvider(Game game)
		{
			this.game = game;
			this.game.Content.RootDirectory = "Content";
			this.DefaultContent = new DefaultContent(this.game.Content);

			// this.Load<SoundEffect>("");
		}

		public Texture2D GenerateTexture(int width, int height, bool mipmap = true)
		{
			return new Texture2D(this.game.GraphicsDevice, width, height, mipmap, SurfaceFormat.Color);
		}

		public virtual T Load<T>(string assetName)
		{
			Type t = typeof(T);

			if (!this.audioEnabled && t == typeof(SoundEffect) || t == typeof(Song))
				return default;

			try
			{
				if(this.cache.TryGetValue(assetName, out object cachedAsset) && cachedAsset is T typedAsset)
					return typedAsset;
				
				T asset = this.game.Content.Load<T>(assetName);
				this.cache[assetName] = asset;
				return asset;
			}
			catch(NoAudioHardwareException)
			{
				this.audioEnabled = false;
				return default;
			}
		}

		public virtual T LoadLocalized<T>(string assetName)
		{
			Type t = typeof(T);

			if (!this.audioEnabled && t == typeof(SoundEffect) || t == typeof(Song))
				return default;

			try
			{
				if (this.cache.TryGetValue(assetName, out object cachedAsset) && cachedAsset is T typedAsset)
					return typedAsset;

				T asset = this.game.Content.LoadLocalized<T>(assetName);
				this.cache[assetName] = asset;
				return asset;

			}
			catch (NoAudioHardwareException)
			{
				this.audioEnabled = false;
				return default;
			}
		}
	}
}
