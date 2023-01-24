using Myra.Graphics2D.UI;

namespace Frozen.ECS.Components
{
	public class DebugUI : UI
	{
		private Label _fps;

		private Label _mouse;

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

			_fps = new Label() { GridRow = 0 };
			grid.Widgets.Add(_fps);

			_mouse = new Label() { GridRow = 1 };
			grid.Widgets.Add(_mouse);

			return new Desktop { Root = grid };
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			_fps.Text = $"fps: {1 / Time.FrameSeconds:0.000}";
			_mouse.Text = Engine.Mouse.Position.ToString();
		}
	}
}
