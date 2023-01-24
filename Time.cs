using System;
using Microsoft.Xna.Framework;

namespace Frozen
{
	public static class Time
	{
		public static float TimeScale { get; private set; } = 1;
		public static float FrameMilliseconds { get; private set; }
		public static float FrameSeconds { get; private set; }
		public static bool IsRunningSlowly { get; private set; }
		public static TimeSpan TotalGameTime { get; private set; }
		public static float TotalGameSeconds => (float)TotalGameTime.TotalSeconds;
		public static TimeSpan ScaledGameTime { get; private set; }
		public static float ScaledGameSeconds => (float)ScaledGameTime.TotalSeconds;

		static Time()
		{
			TotalGameTime = TimeSpan.FromSeconds(0);
			ScaledGameTime = TimeSpan.FromSeconds(0);
		}

		public static void SetTimeScale(float scale)
		{
			TimeScale = scale;
		}

		internal static void Update(GameTime gameTime)
		{
			TotalGameTime = gameTime.TotalGameTime;
			FrameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			FrameMilliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			ScaledGameTime += TimeSpan.FromSeconds(FrameSeconds * TimeScale);

			IsRunningSlowly = gameTime.IsRunningSlowly;
		}
	}
}
