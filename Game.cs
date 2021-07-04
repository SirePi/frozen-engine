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
	public abstract class Game : Microsoft.Xna.Framework.Game
	{
		protected abstract Scene StartingScene { get; }
		internal Scene CurrentScene { get; private set; }
		private Scene nextScene;
		protected GraphicsDeviceManager graphics;

		protected Game()
		{
			this.graphics = new GraphicsDeviceManager(this);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Frozen.Initialize(this);

			this.nextScene = this.StartingScene;
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Frozen.Audio.Update(gameTime);
			Frozen.Keyboard.Update();
			Frozen.Mouse.Update();

			foreach (GamePadManager gamePad in Frozen.GamePad.Values)
				gamePad.Update();

			if (this.nextScene != null)
			{
				Scene current = this.CurrentScene;

				current?.BeforeSwitchingFrom();
				this.nextScene.BeforeSwitchingTo();

				this.CurrentScene = this.nextScene;

				current?.AfterSwitchingFrom();
				this.nextScene.AfterSwitchingTo();

				this.nextScene = null;
			}

			this.CurrentScene.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			Frozen.Drawing.DrawScene(this.CurrentScene, gameTime);
		}

		internal void ChangeScene(Scene nextScene)
		{
			this.nextScene = nextScene;
		}
	}
}
