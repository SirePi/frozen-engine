using Frozen.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.Input
{
	public class MouseManager
	{
		private MouseState lastFrameState;
		private MouseState currentFrameState;

		internal MouseManager()
		{
			this.currentFrameState = Mouse.GetState();
		}

		internal void Update()
		{
			this.lastFrameState = this.currentFrameState;
			this.currentFrameState = Mouse.GetState();
		}

		public bool IsButtonUp(MouseButton key)
		{
			return key.GetButtonState(this.currentFrameState) == ButtonState.Released;
		}

		public bool IsButtonDown(MouseButton key)
		{
			return key.GetButtonState(this.currentFrameState) == ButtonState.Pressed;
		}

		public bool IsButtonHit(MouseButton key)
		{
			return key.GetButtonState(this.currentFrameState) == ButtonState.Pressed && key.GetButtonState(this.lastFrameState) == ButtonState.Released;
		}

		public bool IsButtonReleased(MouseButton key)
		{
			return key.GetButtonState(this.currentFrameState) == ButtonState.Released && key.GetButtonState(this.lastFrameState) == ButtonState.Pressed;
		}

		public Point Delta => this.currentFrameState.Position - this.lastFrameState.Position;
		public Point Position => this.currentFrameState.Position;
		public int HorizontalScrollWheelDelta => this.currentFrameState.HorizontalScrollWheelValue - this.lastFrameState.HorizontalScrollWheelValue;
		public int ScrollWheelDelta => this.currentFrameState.ScrollWheelValue - this.lastFrameState.ScrollWheelValue;
	}
}