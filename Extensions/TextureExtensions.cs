using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Frozen
{
	public static class Texture2DExtensions
	{
		public static void CreateMipMaps(this Texture2D texture)
		{
			int w = texture.Width;
			int h = texture.Height;

			Rgba32[] data = new Rgba32[w * h];
			texture.GetData(0, texture.Bounds, data, 0, data.Length);

			Image<Rgba32> source = Image.LoadPixelData(data, w, h);
			
			for (int i = 1; i < texture.LevelCount; i++)
			{
				w /= 2;
				h /= 2;

				Image<Rgba32> mipmap = source.Clone();
				mipmap.Mutate(x => x.Resize(w, h));
				Rgba32[] mmData = new Rgba32[w * h];

				mipmap.CopyPixelDataTo(mmData);
				texture.SetData(i, new Microsoft.Xna.Framework.Rectangle(0, 0, w, h) , mmData, 0, w * h);
			}
		}
	}
}
