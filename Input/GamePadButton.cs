using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.Input
{
	public enum GamePadButton : byte
	{
		A,
		B,
		Back,
		BigButton,
		LeftShoulder,
		LeftStick,
		RightShoulder,
		RightStick,
		Start,
		X,
		Y
	}

	internal static class GamePadButtonExtension
	{
		internal static ButtonState GetButtonState(this GamePadButton key, GamePadState state)
		{
			switch (key)
			{
				case GamePadButton.A: return state.Buttons.A;
				case GamePadButton.B: return state.Buttons.B;
				case GamePadButton.Back: return state.Buttons.Back;
				case GamePadButton.BigButton: return state.Buttons.BigButton;
				case GamePadButton.LeftShoulder: return state.Buttons.LeftShoulder;
				case GamePadButton.LeftStick: return state.Buttons.LeftStick;
				case GamePadButton.RightShoulder: return state.Buttons.RightShoulder;
				case GamePadButton.RightStick: return state.Buttons.RightStick;
				case GamePadButton.Start: return state.Buttons.Start;
				case GamePadButton.X: return state.Buttons.X;
				case GamePadButton.Y: return state.Buttons.Y;
				default: return ButtonState.Released;
			}
		}
	}

	public enum DPad
	{
		Up,
		Down,
		Left,
		Right
	}

	internal static class GamePadDPadExtension
	{
		internal static ButtonState GetDPadState(this DPad dpad, GamePadState state)
		{
			switch (dpad)
			{
				case DPad.Up: return state.DPad.Up;
				case DPad.Down: return state.DPad.Down;
				case DPad.Left: return state.DPad.Left;
				case DPad.Right: return state.DPad.Right;
				default: return ButtonState.Released;
			}
		}
	}
}