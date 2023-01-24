using Microsoft.Xna.Framework.Input;

namespace Frozen.Input
{
	public class KeyboardManager
	{
		private KeyboardState _currentFrameState;
		private KeyboardState _lastFrameState;

		internal KeyboardManager()
		{
			_currentFrameState = Keyboard.GetState();
		}

		internal void Update()
		{
			_lastFrameState = _currentFrameState;
			_currentFrameState = Keyboard.GetState();
		}

		public bool IsDown(Keys key)
		{
			return _currentFrameState[key] == KeyState.Down;
		}

		public bool IsHit(Keys key)
		{
			return _currentFrameState[key] == KeyState.Down && _lastFrameState[key] == KeyState.Up;
		}

		public bool IsReleased(Keys key)
		{
			return _currentFrameState[key] == KeyState.Up && _lastFrameState[key] == KeyState.Down;
		}

		public bool IsUp(Keys key)
		{
			return _currentFrameState[key] == KeyState.Up;
		}
	}
}
