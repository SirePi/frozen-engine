using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frozen
{
	public static class ColorExtensions
	{
		// Adapted from https://stackoverflow.com/a/1335465/1489138
		public static Color FromHSV(float h, float S, float V)
		{
			float H = h;

			while (H < 0) 
				H += 360;

			while (H >= 360) 
				H -= 360;

			float R, G, B;

			if (V <= 0)
				R = G = B = 0;
			else if (S <= 0)
				R = G = B = V;
			
			else
			{
				float hf = H / 60;
				int i = (int)MathF.Floor(hf);

				float f = hf - i;
				float pv = V * (1 - S);
				float qv = V * (1 - S * f);
				float tv = V * (1 - S * (1 - f));

				switch (i)
				{
					// Red is the dominant color
					case 0:
						R = V;
						G = tv;
						B = pv;
						break;

					// Green is the dominant color
					case 1:
						R = qv;
						G = V;
						B = pv;
						break;
					case 2:
						R = pv;
						G = V;
						B = tv;
						break;

					// Blue is the dominant color
					case 3:
						R = pv;
						G = qv;
						B = V;
						break;
					case 4:
						R = tv;
						G = pv;
						B = V;
						break;

					// Red is the dominant color
					case 5:
						R = V;
						G = pv;
						B = qv;
						break;

					// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
					case 6:
						R = V;
						G = tv;
						B = pv;
						break;
					case -1:
						R = V;
						G = pv;
						B = qv;
						break;

					// The color is not defined, we should throw an error.
					default:
						R = G = B = V; // Just pretend its black/white
						break;
				}
			}

			return new Color(R, G, B);
		}

		public static Color WithAlpha(this Color color, byte alpha)
		{
			Color result = color;
			result.A = alpha;
			return result;
		}

		public static Color WithAlpha(this Color color, float alpha)
		{
			Color result = color;
			result.A = (byte)(alpha * 255f);
			return result;
		}

		public static float[] ToFloatArray(this Color color)
		{
			return new float[] { color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f };
		}
	}
}
