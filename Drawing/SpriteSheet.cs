using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FrozenEngine.Drawing
{
	public class SpriteSheet
	{
		public Texture2D Texture { get; set; }
		public SpriteAtlas Atlas { get; private set; }
	}
}
