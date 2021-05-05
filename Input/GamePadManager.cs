using FrozenEngine.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.Input
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

		public InputState this[GamePadButton key]
		{
			get
			{
				if (key.GetButtonState(this.currentFrameState) == ButtonState.Released) return InputState.Up;
				else if (key.GetButtonState(this.lastFrameState) == ButtonState.Released) return InputState.Hit;
				else return InputState.Held;
			}
		}

		public InputState this[DPad dpad]
		{
			get
			{
				if (dpad.GetDPadState(this.currentFrameState) == ButtonState.Released) return InputState.Up;
				else if (dpad.GetDPadState(this.lastFrameState) == ButtonState.Released) return InputState.Hit;
				else return InputState.Held;
			}
		}
	}
}