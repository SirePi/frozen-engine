using Frozen.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.Input
{
	public class KeyboardManager
	{
		private KeyboardState lastFrameState;
		private KeyboardState currentFrameState;

		internal KeyboardManager()
		{
			this.currentFrameState = Keyboard.GetState();
		}

		internal void Update()
		{
			this.lastFrameState = this.currentFrameState;
			this.currentFrameState = Keyboard.GetState();
		}

		public bool IsUp(Keys key)
		{
			return this.currentFrameState[key] == KeyState.Up;
		}

		public bool IsDown(Keys key)
		{
			return this.currentFrameState[key] == KeyState.Down;
		}

		public bool IsHit(Keys key)
		{
			return this.currentFrameState[key] == KeyState.Down && this.lastFrameState[key] == KeyState.Up;
		}

		public bool IsReleased(Keys key)
		{
			return this.currentFrameState[key] == KeyState.Up && this.lastFrameState[key] == KeyState.Down;
		}
	}
}
