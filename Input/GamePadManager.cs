using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Frozen.Input
{
	public class GamePadManager
	{
		private readonly PlayerIndex _player;

		private GamePadState _currentFrameState;
		private GamePadState _lastFrameState;

		internal GamePadManager(PlayerIndex player)
		{
			_player = player;
			_currentFrameState = GamePad.GetState(player);
		}

		internal void Update()
		{
			_lastFrameState = _currentFrameState;
			_currentFrameState = GamePad.GetState(_player, GamePadDeadZone.Circular);
		}

		public bool IsButtonDown(GamePadButton key)
		{
			return key.GetButtonState(_currentFrameState) == ButtonState.Pressed;
		}

		public bool IsButtonHit(GamePadButton key)
		{
			return key.GetButtonState(_currentFrameState) == ButtonState.Pressed && key.GetButtonState(_lastFrameState) == ButtonState.Released;
		}

		public bool IsButtonReleased(GamePadButton key)
		{
			return key.GetButtonState(_currentFrameState) == ButtonState.Released && key.GetButtonState(_lastFrameState) == ButtonState.Pressed;
		}

		public bool IsButtonReleased(DPad key)
		{
			return key.GetDPadState(_currentFrameState) == ButtonState.Released && key.GetDPadState(_lastFrameState) == ButtonState.Pressed;
		}

		public bool IsButtonUp(GamePadButton key)
		{
			return key.GetButtonState(_currentFrameState) == ButtonState.Released;
		}

		public bool IsDPadDown(DPad key)
		{
			return key.GetDPadState(_currentFrameState) == ButtonState.Pressed;
		}

		public bool IsDPadHit(DPad key)
		{
			return key.GetDPadState(_currentFrameState) == ButtonState.Pressed && key.GetDPadState(_lastFrameState) == ButtonState.Released;
		}

		public bool IsDPadUp(DPad key)
		{
			return key.GetDPadState(_currentFrameState) == ButtonState.Released;
		}

		public void SetVibration(float motor)
		{
			SetVibration(motor, motor, 0, 0);
		}

		public void SetVibration(float leftMotor, float rightMotor)
		{
			SetVibration(leftMotor, rightMotor, 0, 0);
		}

		public void SetVibration(float leftMotor, float rightMotor, float leftTrigger, float rightTrigger)
		{
			GamePad.SetVibration(_player, leftMotor, rightMotor, leftTrigger, rightTrigger);
		}
	}
}
