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

		public void Draw()
		{
			_desktop.Render();
		}

		protected abstract Desktop BuildUI();
	}
}
