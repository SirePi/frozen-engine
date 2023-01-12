﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FontStashSharp;
using Frozen.Drawing;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Frozen
{
	public class ContentProvider
	{
		private Dictionary<string, object> cache = new Dictionary<string, object>();
		private Dictionary<string, FontSystem> fontCache = new Dictionary<string, FontSystem>();
		private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

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

		public virtual T LoadXNA<T>(string assetName)
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

		public virtual SpriteFont LoadSpriteFont(string font, int size, params CharacterRange[] characters)
		{
			if (!this.fontCache.TryGetValue(font, out FontSystem fontSystem))
			{
				fontSystem = new FontSystem();
				fontSystem.AddFont(File.ReadAllBytes(font));
				this.fontCache[font] = fontSystem;
			}

			return fontSystem.GetFont(size).ToXNASpriteFont(characters);
		}

		public virtual Texture2D LoadTexture(string texture, bool generateMipmaps = true)
		{
			if (this.textureCache.TryGetValue(texture, out Texture2D tx))
				return tx;

			Image<Rgba32> img = Image.Load<Rgba32>(texture);
			byte[] data = new byte[img.Width * img.Height * img.PixelType.BitsPerPixel / 8];
			img.CopyPixelDataTo(data);

			tx = new Texture2D(Engine.Game.GraphicsDevice, img.Width, img.Height, generateMipmaps, SurfaceFormat.Color);
			tx.SetData(data);

			if (generateMipmaps)
				tx.CreateMipMaps();

			this.textureCache[texture] = tx;
			return tx;
		}

		public virtual SoundEffect LoadSoundEffect(string soundEffect)
		{
			string extension = soundEffect.Split(".").Last().ToLowerInvariant();

			if (extension == "ogg")
			{
				using NVorbis.VorbisReader reader = new NVorbis.VorbisReader(soundEffect);
				float[] data = new float[reader.TotalSamples];

				reader.ReadSamples(data, 0, (int)reader.TotalSamples);
				byte[] x = data.SelectMany(f => BitConverter.GetBytes((short)(f * short.MaxValue))).ToArray();

				return new SoundEffect(x, reader.SampleRate, reader.Channels == 1 ? AudioChannels.Mono : AudioChannels.Stereo);
			}
			else
			{
				return SoundEffect.FromFile(soundEffect);
			}
		}

		public virtual T LoadXNALocalized<T>(string assetName)
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
