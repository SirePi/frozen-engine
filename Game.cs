using Frozen.ECS;
using Frozen.Input;
using Microsoft.Xna.Framework;

namespace Frozen
{
	public abstract class Game : Microsoft.Xna.Framework.Game
	{
		private Scene _nextScene;
		protected GraphicsDeviceManager _graphics;

		protected abstract Scene StartingScene { get; }

		internal Scene CurrentScene { get; private set; }

		protected Game()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			_graphics = new GraphicsDeviceManager(this);
		}

		protected override void Draw(GameTime gameTime)
		{
			Engine.Drawing.DrawScene(CurrentScene);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Engine.Initialize(this);

			_nextScene = StartingScene;
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Environment.Update(gameTime);
			Time.Update(gameTime);

			// Engine.Audio.Update();
			Engine.Keyboard.Update();
			Engine.Mouse.Update();

			foreach (GamePadManager gamePad in Engine.GamePad.Values)
				gamePad.Update();

			if (_nextScene != null)
			{
				Scene current = CurrentScene;

				current?.BeforeSwitchingFrom();
				_nextScene.BeforeSwitchingTo();

				CurrentScene = _nextScene;

				current?.AfterSwitchingFrom();
				_nextScene.AfterSwitchingTo();

				_nextScene = null;
			}

			CurrentScene.Update();
		}

		internal void ChangeScene(Scene nextScene)
		{
			_nextScene = nextScene;
		}
	}
}
