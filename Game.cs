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

		protected override void Initialize()
		{
			base.Initialize();
			System.Initialize(this);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			System.Audio.Update(gameTime);
			System.Keyboard.Update();
			System.Mouse.Update();

			foreach (GamePadManager gamePad in System.GamePad.Values)
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
			System.Drawing.DrawScene(this.CurrentScene, gameTime);
		}

		internal void ChangeScene(Scene nextScene)
		{
			this.nextScene = nextScene;
		}
	}
}
