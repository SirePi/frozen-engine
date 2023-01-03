﻿using System;
using System.Collections.Generic;
using System.IO;
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
			var vorbisStream = new NAudio.Vorbis.VorbisWaveReader(soundEffect);
			var channel = new NAudio.Wave.WaveChannel32(vorbisStream);
			var waveOut = new NAudio.Wave.DirectSoundOut();
			{
				waveOut.Init(channel);
				channel.Volume = 1;
				channel.Pan = -1;
				waveOut.Play();

				int i = 0;
				float delta = .2f;
				while (waveOut.PlaybackState == NAudio.Wave.PlaybackState.Playing)
				{
					System.Threading.Thread.Sleep(100);
					channel.Pan += delta;
					channel.Volume = MathF.Max(0, channel.Volume - .1f);

					if(++i == 8)
					{
						channel.Position = 0;
						channel.Pan = delta * 5;
						channel.Volume = 1;
						delta *= -1;
						i = 0;
					}
				}

				// wait here until playback stops or should stop
			}

			return null;
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
