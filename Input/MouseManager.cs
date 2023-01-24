using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Frozen.Input
{
	public class MouseManager
	{
		private MouseState _currentFrameState;
		private MouseState _lastFrameState;
		public Point Delta => _currentFrameState.Position - _lastFrameState.Position;

		public int HorizontalScrollWheelDelta => _currentFrameState.HorizontalScrollWheelValue - _lastFrameState.HorizontalScrollWheelValue;

		public Point Position => _currentFrameState.Position;

		public int ScrollWheelDelta => _currentFrameState.ScrollWheelValue - _lastFrameState.ScrollWheelValue;

		internal MouseManager()
		{
			_currentFrameState = Mouse.GetState();
		}

		internal void Update()
		{
			_lastFrameState = _currentFrameState;
			_currentFrameState = Mouse.GetState();
		}

		public bool IsButtonDown(MouseButton key)
		{
			return key.GetButtonState(_currentFrameState) == ButtonState.Pressed;
		}

		public bool IsButtonHit(MouseButton key)
		{
			return key.GetButtonState(_currentFrameState) == ButtonState.Pressed && key.GetButtonState(_lastFrameState) == ButtonState.Released;
		}

		public bool IsButtonReleased(MouseButton key)
		{
			return key.GetButtonState(_currentFrameState) == ButtonState.Released && key.GetButtonState(_lastFrameState) == ButtonState.Pressed;
		}

		public bool IsButtonUp(MouseButton key)
		{
			return key.GetButtonState(_currentFrameState) == ButtonState.Released;
		}
	}
}
