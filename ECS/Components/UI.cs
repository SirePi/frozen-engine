using Myra.Graphics2D.UI;

namespace Frozen.ECS.Components
{
	public abstract class UI : Component
	{
		protected readonly Desktop desktop;

		public bool IsMouseOverGUI => this.desktop.IsMouseOverGUI;

		public UI()
		{
#pragma warning disable S1699 // Constructors should only call non-overridable methods
			this.desktop = this.BuildUI();
#pragma warning restore S1699 // Constructors should only call non-overridable methods
		}

		public void Draw()
		{
			this.desktop.Render();
		}

		protected abstract Desktop BuildUI();
	}
}
