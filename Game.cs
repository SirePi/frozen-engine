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

namespace FrozenEngine
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		internal Scene CurrentScene { get; private set; }
		private Scene nextScene;

		protected override void Initialize()
		{
			base.Initialize();
			Core.Initialize(this);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
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
		}

		internal void ChangeScene(Scene nextScene)
		{
			this.nextScene = nextScene;
		}
	}
}
