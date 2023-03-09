using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frozen
{
	public static class Environment
	{
		private static Point lastWindowSize;
		public static bool WindowResized { get; private set; }

		internal static void Update(GameTime gameTime)
		{
			WindowResized = Engine.Game.Window.ClientBounds.Size != lastWindowSize;

			lastWindowSize = Engine.Game.Window.ClientBounds.Size;
		}
	}
}
