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

		protected override Desktop BuildUI()
		{
			var grid = new Grid
			{
				RowSpacing = 8,
				ColumnSpacing = 8
			};

			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.VerticalAlignment = VerticalAlignment.Bottom;

			this.fps = new Label() { GridRow = 0 };
			grid.Widgets.Add(this.fps);

			this.mouse = new Label() { GridRow = 1 };
			grid.Widgets.Add(this.mouse);

			return new Desktop { Root = grid };
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			this.fps.Text = $"fps: {1 / gameTime.ElapsedGameTime.TotalSeconds:0.000}";

			Camera c = this.Entity.Scene.GetCameras().First();
			Vector3 unproj = c.ScreenToWorld(System.Mouse.Position.ToVector2(), 0);
			Vector3 proj = c.WorldToScreen(unproj);

			//Vector2 w2sNear = c.WorldToScreen(Vector3.UnitZ * c.NearPlaneDistance);
			//Vector2 w2sFar = c.WorldToScreen(Vector3.UnitZ * c.FarPlaneDistance);

			this.mouse.Text = $"{System.Mouse.Position} - S2W: ${unproj} - W2S: ${proj}";
			//this.mouse.Text = $"Mouse: {System.Mouse.Position} - Camera: {c.Transform.Position.XY()} - Near: ${w2sNear} - Far: ${w2sFar}";
		}
	}
}
