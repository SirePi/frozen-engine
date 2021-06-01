using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using FrozenEngine.ECS;
using FrozenEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FrozenEngine.ECS.Systems;

namespace FrozenEngine
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		internal Scene CurrentScene { get; private set; }
		private Scene nextScene;

		protected AudioSystem Audio => Core.Audio;
		protected KeyboardManager Keyboard { get; private set; }
		protected IReadOnlyDictionary<PlayerIndex, GamePadManager> GamePad { get; private set; }

		protected override void Initialize()
		{
			base.Initialize();

			Core.Initialize(this);

			this.Keyboard = new KeyboardManager();
			this.GamePad = Enum.GetValues(typeof(PlayerIndex)).Cast<PlayerIndex>().ToDictionary(k => k, v => new GamePadManager(v));
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			this.Keyboard.Update();
			foreach (GamePadManager gamePad in this.GamePad.Values)
				gamePad.Update();

			if (this.nextScene != null)
			{
				this.CurrentScene = this.nextScene;
				this.nextScene = null;
			}

			this.CurrentScene.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			Core.Drawing.DrawScene(this.CurrentScene, gameTime);
			Core.Audio.Update(gameTime);
		}

		internal void ChangeScene(Scene nextScene)
		{
			this.nextScene = nextScene;
		}
	}
}
