using System;
using System.Collections.Generic;
using System.IO;
using FontStashSharp;
using Frozen.Audio;
using Frozen.Drawing;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SkiaSharp;

namespace Frozen
{
	public class ContentProvider
	{
		private Dictionary<string, AudioSource> _audioCache = new Dictionary<string, AudioSource>();
		private bool _audioEnabled = true;
		private Dictionary<string, object> _cache = new Dictionary<string, object>();
		private Dictionary<string, FontSystem> _fontCache = new Dictionary<string, FontSystem>();
		private Game _game;
		private Dictionary<string, SkiaSharp.Extended.Svg.SKSvg> _svgCache = new Dictionary<string, SkiaSharp.Extended.Svg.SKSvg>();
		private Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();

		internal DefaultContent DefaultContent { get; private set; }

		internal ContentProvider(Game game)
		{
			_game = game;
			_game.Content.RootDirectory = "Content";
			DefaultContent = new DefaultContent(_game.Content);
		}

		public Texture2D GenerateTexture(int width, int height, bool mipmap = true)
		{
			return new Texture2D(_game.GraphicsDevice, width, height, mipmap, SurfaceFormat.Color);
		}

		public AudioSource LoadAudio(string audio)
		{
			if (!_audioCache.TryGetValue(audio, out AudioSource source))
			{
				source = new FileAudioSource(audio);
				_audioCache[audio] = source;
			}

			return source;
		}

		public SpriteFontBase LoadSpriteFont(string font, int size)
		{
			return LoadSpriteFont(font, size, CharacterRange.Default);
		}

		public SpriteFontBase LoadSpriteFont(string font, int size, params CharacterRange[] characters)
		{
			if (!_fontCache.TryGetValue(font, out FontSystem fontSystem))
			{
				fontSystem = new FontSystem();
				fontSystem.AddFont(File.ReadAllBytes(font));
				_fontCache[font] = fontSystem;
			}

			return fontSystem.GetFont(size);
		}

		public Texture2D LoadTexture(string texture, bool generateMipmaps = true)
		{
			if (_textureCache.TryGetValue(texture, out Texture2D tx))
				return tx;

			byte[] src = File.ReadAllBytes(texture);
			using SKBitmap bmp = SKBitmap.Decode(src);

			tx = SKBitmapToTexture2D(bmp, generateMipmaps);

			_textureCache[texture] = tx;
			return tx;
		}

		public Texture2D LoadSvg(string svg, int width, int? height = null, bool generateMipmaps = true)
		{
			if (!_svgCache.TryGetValue(svg, out SkiaSharp.Extended.Svg.SKSvg sksvg))
			{
				sksvg = new SkiaSharp.Extended.Svg.SKSvg();
				sksvg.Load(svg);
				_svgCache[svg] = sksvg;
			}

			if(height == null)
				height = (int)MathF.Ceiling(width * sksvg.Picture.CullRect.Height / sksvg.Picture.CullRect.Width);

			string textureId = $"{svg}:{width}x{height}";

			if (_textureCache.TryGetValue(textureId, out Texture2D tx))
				return tx;

			SKMatrix scale = SKMatrix.CreateScale(width / sksvg.Picture.CullRect.Width, height.Value / sksvg.Picture.CullRect.Height);
			using SKBitmap bmp = new SKBitmap(new SKImageInfo(width, height.Value, SKColorType.Rgba8888));
			using SKCanvas canvas = new SKCanvas(bmp);

			canvas.Clear(SKColors.Transparent);
			canvas.DrawPicture(sksvg.Picture, ref scale);
			canvas.Flush();

			tx = SKBitmapToTexture2D(bmp, generateMipmaps);

			_textureCache[textureId] = tx;
			return tx;
		}

		private Texture2D SKBitmapToTexture2D(SKBitmap bitmap, bool generateMipmaps)
		{
			Texture2D texture = new Texture2D(Engine.Game.GraphicsDevice, bitmap.Width, bitmap.Height, generateMipmaps, SurfaceFormat.Color);
			texture.SetData(bitmap.Pixels.ToXNA());

			if (generateMipmaps)
				texture.CreateMipMaps();

			return texture;
		}

		public T LoadXNA<T>(string assetName)
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

		public T LoadXNALocalized<T>(string assetName)
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
