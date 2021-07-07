using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.Input
{
	public enum MouseButton : byte
	{
		LeftButton,
		MiddleButton,
		RightButton,
		XButton1,
		XButton2
	}

	internal static class MouseButtonExtension
	{
		internal static ButtonState GetButtonState(this MouseButton key, MouseState state)
		{
			switch (key)
			{
				case MouseButton.LeftButton: return state.LeftButton;
				case MouseButton.MiddleButton: return state.MiddleButton;
				case MouseButton.RightButton: return state.RightButton;
				case MouseButton.XButton1: return state.XButton1;
				case MouseButton.XButton2: return state.XButton2;
				default: return ButtonState.Released;
			}
		}
	}
}