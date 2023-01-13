using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using Frozen.ECS;
using Frozen.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Frozen.ECS.Systems;

namespace Frozen
{
	public abstract class Game : Microsoft.Xna.Framework.Game
	{
		protected abstract Scene StartingScene { get; }
		internal Scene CurrentScene { get; private set; }
		private Scene nextScene;
		protected GraphicsDeviceManager graphics;

		protected Game()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			this.graphics = new GraphicsDeviceManager(this);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Engine.Initialize(this);

			this.nextScene = this.StartingScene;
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			Time.Update(gameTime);

			Engine.Audio.Update();
			Engine.Keyboard.Update();
			Engine.Mouse.Update();

			foreach (GamePadManager gamePad in Engine.GamePad.Values)
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

			this.CurrentScene.Update();
		}

		protected override void Draw(GameTime gameTime)
		{
			Engine.Drawing.DrawScene(this.CurrentScene);
		}

		internal void ChangeScene(Scene nextScene)
		{
			this.nextScene = nextScene;
		}
	}
}
