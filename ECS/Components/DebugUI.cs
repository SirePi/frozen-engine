using Myra.Graphics2D.UI;

namespace Frozen.ECS.Components
{
	public class DebugUI : UI
	{
		private Label fps;

		private Label mouse;

		private Label orbiter;

		protected override Desktop BuildUI()
		{
			var grid = new Grid
			{
				RowSpacing = 8,
				ColumnSpacing = 8
			};

			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.VerticalAlignment = VerticalAlignment.Bottom;

			this.fps = new Label() { GridRow = 0 };
			grid.Widgets.Add(this.fps);

			this.mouse = new Label() { GridRow = 1 };
			grid.Widgets.Add(this.mouse);

			this.orbiter = new Label { GridRow = 2 };
			grid.Widgets.Add(this.orbiter);

			return new Desktop { Root = grid };
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.fps.Text = $"fps: {1 / Time.FrameSeconds:0.000}";
			this.mouse.Text = Engine.Mouse.Position.ToString();
		}
	}
}
