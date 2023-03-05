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

			SKColor[] data = new SKColor[w * h];
			texture.GetData(0, texture.Bounds, data, 0, data.Length);

			using SKBitmap bmp = new SKBitmap(w, h);
			bmp.Pixels = data;

			for (int i = 1; i < texture.LevelCount; i++)
			{
				w /= 2;
				h /= 2;

				using SKBitmap mipmap = new SKBitmap(w, h);

				bmp.ScalePixels(mipmap, SKFilterQuality.High);
				texture.SetData(i, new Microsoft.Xna.Framework.Rectangle(0, 0, w, h), mipmap.Pixels, 0, w * h);
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
