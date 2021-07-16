using Frozen.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.Input
{
	public class GamePadManager
	{
		private readonly PlayerIndex player;

		private GamePadState lastFrameState;
		private GamePadState currentFrameState;

		internal GamePadManager(PlayerIndex player)
		{
			this.player = player;
			this.currentFrameState = GamePad.GetState(this.player);
		}

		internal void Update()
		{
			this.lastFrameState = this.currentFrameState;
			this.currentFrameState = GamePad.GetState(this.player, GamePadDeadZone.Circular);
		}

		public bool IsButtonUp(GamePadButton key)
		{
			return key.GetButtonState(this.currentFrameState) == ButtonState.Released;
		}

		public bool IsButtonDown(GamePadButton key)
		{
			return key.GetButtonState(this.currentFrameState) == ButtonState.Pressed;
		}

		public bool IsButtonHit(GamePadButton key)
		{
			return key.GetButtonState(this.currentFrameState) == ButtonState.Pressed && key.GetButtonState(this.lastFrameState) == ButtonState.Released;
		}

		public bool IsButtonReleased(GamePadButton key)
		{
			return key.GetButtonState(this.currentFrameState) == ButtonState.Released && key.GetButtonState(this.lastFrameState) == ButtonState.Pressed;
		}

		public bool IsDPadUp(DPad key)
		{
			return key.GetDPadState(this.currentFrameState) == ButtonState.Released;
		}

		public bool IsDPadDown(DPad key)
		{
			return key.GetDPadState(this.currentFrameState) == ButtonState.Pressed;
		}

		public bool IsDPadHit(DPad key)
		{
			return key.GetDPadState(this.currentFrameState) == ButtonState.Pressed && key.GetDPadState(this.lastFrameState) == ButtonState.Released;
		}

		public bool IsButtonReleased(DPad key)
		{
			return key.GetDPadState(this.currentFrameState) == ButtonState.Released && key.GetDPadState(this.lastFrameState) == ButtonState.Pressed;
		}
	}
}