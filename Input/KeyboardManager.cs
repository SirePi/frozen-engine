using FrozenEngine.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.Input
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

		public InputState this[Keys key]
		{
			get
			{
				if (this.currentFrameState[key] == KeyState.Up) return InputState.Up;
				else if (this.lastFrameState[key] == KeyState.Up) return InputState.Hit;
				else return InputState.Held;
			}
		}
	}
}
