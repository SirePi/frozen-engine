using System;

namespace Frozen.Coroutines
{
	/// <summary>
	/// Struct in charge of managing the timings of a Coroutine's execution
	/// </summary>
	public struct WaitUntil
	{
		private enum WaitType
		{
			Frames,
			GameTime,
			RealTime
		}

		private readonly WaitType _type;
		private float _internalValue;

		/// <summary>
		/// Waits until the next frame
		/// </summary>
		public static readonly WaitUntil NextFrame = new WaitUntil(1, WaitType.Frames);

		public bool IsComplete
		{
			get { return _internalValue <= 0; }
		}

		private WaitUntil(float startingValue, WaitType type)
		{
			_internalValue = startingValue;
			_type = type;
		}

		internal void Update()
		{
			switch (_type)
			{
				case WaitType.Frames:
					_internalValue -= 1;
					break;

				case WaitType.GameTime:
					_internalValue -= Time.FrameSeconds;
					break;

				case WaitType.RealTime:
					_internalValue -= Time.FrameSeconds;
					break;
			}
		}

		/// <summary>
		/// Waits until the desired number of frames
		/// </summary>
		/// <param name="frames">The number of frames to wait</param>
		/// <returns>A new WaitUntil struct</returns>
		public static WaitUntil Frames(int frames)
		{
			return new WaitUntil(frames, WaitType.Frames);
		}

		/// <summary>
		/// Waits until the desired number of seconds
		/// </summary>
		/// <param name="seconds">The amount of seconds to wait</param>
		/// <param name="realTime">If true, the countdown is made based on real time, game time (default) otherwise</param>
		/// <returns>A new WaitUntil struct</returns>
		public static WaitUntil Seconds(float seconds, bool realTime = false)
		{
			return new WaitUntil(seconds, realTime ? WaitType.RealTime : WaitType.GameTime);
		}

		/// <summary>
		/// Waits until the desired amount of time
		/// </summary>
		/// <param name="timeSpan">The amount of time to wait</param>
		/// <param name="realTime">If true, the countdown is made based on real time, game time (default) otherwise</param>
		/// <returns>A new WaitUntil struct</returns>
		public static WaitUntil TimeSpan(TimeSpan timeSpan, bool realTime = false)
		{
			return WaitUntil.Seconds((float)timeSpan.TotalSeconds, realTime);
		}
	}
}
