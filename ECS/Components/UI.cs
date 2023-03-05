using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace Frozen.ECS.Components
{
	public abstract class UI : Component
	{
		protected readonly Desktop _desktop;

		public bool IsMouseOverGUI => _desktop.IsMouseOverGUI;

		public UI()
		{
#pragma warning disable S1699 // Constructors should only call non-overridable methods
			_desktop = BuildUI();
#pragma warning restore S1699 // Constructors should only call non-overridable methods
		}

		/*
		public void Draw(DrawingSystem drawing)
		{
			if(_renderTarget == null || _renderTarget.Bounds != Myra.MyraEnvironment.GraphicsDevice.Viewport.Bounds)
				_renderTarget = new RenderTarget2D(Myra.MyraEnvironment.GraphicsDevice, Myra.MyraEnvironment.GraphicsDevice.Viewport.Width, Myra.MyraEnvironment.GraphicsDevice.Viewport.Height);

			Myra.MyraEnvironment.GraphicsDevice.SetRenderTarget(_renderTarget);
			_desktop.Render();
			Myra.MyraEnvironment.GraphicsDevice.SetRenderTarget(null);

			drawing.Batch.Begin();
			drawing.Batch.Draw(_renderTarget, drawing.Batch.GraphicsDevice.Viewport.Bounds, Color.White);
			drawing.Batch.End();
		}
		*/

		public void Draw()
		{
			_desktop.Render();
		}

		protected abstract Desktop BuildUI();
	}
}
