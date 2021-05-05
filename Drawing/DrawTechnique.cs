using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.Drawing
{
	public class DrawTechnique
	{
		internal Effect Effect { get; private set; }
		internal bool IsCompiled { get; private set; }

		internal void Compile(GraphicsDevice device)
		{
			this.Effect = new BasicEffect(device) { Alpha = .5f };
			this.IsCompiled = true;
		}
	}
}
