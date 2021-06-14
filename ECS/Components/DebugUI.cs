using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui.Standard;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.ECS.Components
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

		protected override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			this.fps.Text = $"fps: {1 / gameTime.ElapsedGameTime.TotalSeconds:0.000}";

			TwoPointFiveDCamera c = this.Entity.Scene.GetComponents<TwoPointFiveDCamera>().First() ;
			Vector3 unproj = c.ScreenToWorld(Frozen.Mouse.Position.ToVector2(), 0);
			Vector3 unproj2 = c.ScreenToWorld(Frozen.Mouse.Position.ToVector2(), 100);
			Vector3 proj = c.WorldToScreen(unproj);

			Entity orbiter = this.Entity.Scene.GetEntityByName("delta");

			//Vector2 w2sNear = c.WorldToScreen(Vector3.UnitZ * c.NearPlaneDistance);
			//Vector2 w2sFar = c.WorldToScreen(Vector3.UnitZ * c.FarPlaneDistance);

			this.mouse.Text = $"{Frozen.Mouse.Position} - S2W: {unproj} - S2W: {unproj2} - W2S: {proj} - PixelZ : {c.PixelPerfectPlane}";
			this.orbiter.Text = $"{orbiter.Get<Transform>().WorldPosition}";
			//this.mouse.Text = $"Mouse: {System.Mouse.Position} - Camera: {c.Transform.Position.XY()} - Near: ${w2sNear} - Far: ${w2sFar}";
		}
	}
}
