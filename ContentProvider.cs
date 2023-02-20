using System;
using System.Collections.Generic;
using System.IO;
using FontStashSharp;
using Frozen.Audio;
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
		private Dictionary<string, AudioSource> _audioCache = new Dictionary<string, AudioSource>();
		private bool _audioEnabled = true;
		private Dictionary<string, object> _cache = new Dictionary<string, object>();
		private Dictionary<string, FontSystem> _rawFontCache = new Dictionary<string, FontSystem>();
		private Dictionary<string, Dictionary<int, Font>> _fontCache = new Dictionary<string, Dictionary<int, Font>>();
		private Game _game;
		private Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();

		internal DefaultContent DefaultContent { get; private set; }

		internal ContentProvider(Game game)
		{
			_game = game;
			_game.Content.RootDirectory = "Content";
			DefaultContent = new DefaultContent(_game.Content);

			// Load<SoundEffect>("");
		}

		public Texture2D GenerateTexture(int width, int height, bool mipmap = true)
		{
			return new Texture2D(_game.GraphicsDevice, width, height, mipmap, SurfaceFormat.Color);
		}

		public virtual AudioSource LoadAudio(string audio)
		{
			if (!_audioCache.TryGetValue(audio, out AudioSource source))
			{
				source = new FileAudioSource(audio);
				_audioCache[audio] = source;
			}

			return source;
		}

		public virtual SpriteFont LoadSpriteFont(string font, int size)
		{
			return LoadSpriteFont(font, size, CharacterRange.Default);
		}

		public virtual SpriteFont LoadSpriteFont(string font, int size, params CharacterRange[] characters)
		{
			if (!_rawFontCache.TryGetValue(font, out FontSystem fontSystem))
			{
				fontSystem = new FontSystem();
				fontSystem.AddFont(File.ReadAllBytes(font));
				_rawFontCache[font] = fontSystem;
			}

			if (!_fontCache.ContainsKey(font))
				_fontCache[font] = new Dictionary<int, Font>();

			if (!_fontCache[font].ContainsKey(size))
				_fontCache[font][size] = new Font(fontSystem.GetFont(size).ToXNASpriteFont(characters));

			return _fontCache[font][size];
		}

		public virtual Texture2D LoadTexture(string texture, bool generateMipmaps = true)
		{
			if (_textureCache.TryGetValue(texture, out Texture2D tx))
				return tx;

			Image<Rgba32> img = Image.Load<Rgba32>(texture);
			byte[] data = new byte[img.Width * img.Height * img.PixelType.BitsPerPixel / 8];
			img.CopyPixelDataTo(data);

			tx = new Texture2D(Engine.Game.GraphicsDevice, img.Width, img.Height, generateMipmaps, SurfaceFormat.Color);
			tx.SetData(data);

			if (generateMipmaps)
				tx.CreateMipMaps();

			_textureCache[texture] = tx;
			return tx;
		}

		public virtual T LoadXNA<T>(string assetName)
		{
			Type t = typeof(T);

			if (!_audioEnabled && t == typeof(SoundEffect) || t == typeof(Song))
				return default;

			try
			{
				if (_cache.TryGetValue(assetName, out object cachedAsset) && cachedAsset is T typedAsset)
					return typedAsset;

				T asset = _game.Content.Load<T>(assetName);
				_cache[assetName] = asset;
				return asset;
			}
			catch (NoAudioHardwareException)
			{
				_audioEnabled = false;
				return default;
			}
		}

		public virtual T LoadXNALocalized<T>(string assetName)
		{
			Type t = typeof(T);

			if (!_audioEnabled && t == typeof(SoundEffect) || t == typeof(Song))
				return default;

			try
			{
				if (_cache.TryGetValue(assetName, out object cachedAsset) && cachedAsset is T typedAsset)
					return typedAsset;

				T asset = _game.Content.LoadLocalized<T>(assetName);
				_cache[assetName] = asset;
				return asset;
			}
			catch (NoAudioHardwareException)
			{
				_audioEnabled = false;
				return default;
			}
		}
	}
}
