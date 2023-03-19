using System;
using Microsoft.Xna.Framework.Graphics;
using SkiaSharp;

namespace Frozen
{
	public static class Texture2DExtensions
	{
		public static void CreateMipMaps(this Texture2D texture)
		{
			int w = texture.Width;
			int h = texture.Height;

			SKColor[] data = new SKColor[texture.Width * texture.Height];
			texture.GetData(0, texture.Bounds, data, 0, data.Length);

			using SKBitmap bmp = new SKBitmap(texture.Width, texture.Height);
			bmp.Pixels = data;

			for (int i = 1; i < texture.LevelCount; i++)
			{
				w = Math.Max(w >> 1, 1);
				h = Math.Max(h >> 1, 1);

				using SKBitmap mipmap = new SKBitmap(w, h);

				bmp.ScalePixels(mipmap, SKFilterQuality.High);
				texture.SetData(i, new Microsoft.Xna.Framework.Rectangle(0, 0, mipmap.Width, mipmap.Height), mipmap.Pixels, 0, mipmap.Pixels.Length);
			}
		}

		public static void SaveAsJpeg(this Texture2D texture, string path)
		{
			using System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate);
			texture.SaveAsJpeg(fs, texture.Width, texture.Height);
			fs.Flush();
			fs.Close();
		}

		public static void SaveAsPng(this Texture2D texture, string path)
		{
			using System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate);
			texture.SaveAsPng(fs, texture.Width, texture.Height);
			fs.Flush();
			fs.Close();
		}
	}
}
