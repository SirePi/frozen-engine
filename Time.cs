using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frozen
{
	public static class Time
	{
		public static TimeSpan TotalGameTime { get; private set; }
		public static float FrameSeconds { get; private set; }
		public static float FrameMilliseconds { get; private set; }
		public static bool IsRunningSlowly { get; private set; }

		static Time()
		{
			TotalGameTime = TimeSpan.FromSeconds(0);
		}

		internal static void Update(GameTime gameTime)
		{
			TotalGameTime = gameTime.TotalGameTime;
			FrameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			FrameMilliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			IsRunningSlowly = gameTime.IsRunningSlowly;
		}
	}
}
