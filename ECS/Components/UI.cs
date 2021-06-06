using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui.Standard;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS.Components
{
	public class UI : Component
	{
		private readonly ImGUIRenderer ui;
		private readonly Desktop desktop;

		private readonly Action<UI, GameTime> drawMethod;

		public UI(Action<UI, GameTime> drawMethod)
		{
			this.drawMethod = drawMethod;
			this.ui = new ImGUIRenderer(Core.Game).Initialize().RebuildFontAtlas();

			this.desktop = new Desktop();


			MyraEnvironment.Game = Core.Game;

			var grid = new Grid
			{
				RowSpacing = 8,
				ColumnSpacing = 8
			};

			grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

			var helloWorld = new Label
			{
				Id = "label",
				Text = "Hello, World!"
			};
			grid.Widgets.Add(helloWorld);

			// ComboBox
			var combo = new ComboBox
			{
				GridColumn = 1,
				GridRow = 0
			};

			combo.Items.Add(new ListItem("Red", Color.Red));
			combo.Items.Add(new ListItem("Green", Color.Green));
			combo.Items.Add(new ListItem("Blue", Color.Blue));
			grid.Widgets.Add(combo);

			// Button
			var button = new TextButton
			{
				GridColumn = 0,
				GridRow = 1,
				Text = "Show"
			};

			button.Click += (s, a) =>
			{
				var messageBox = Dialog.CreateMessageBox("Message", "Some message!");
				messageBox.ShowModal(this.desktop);
			};

			grid.Widgets.Add(button);

			// Spin button
			var spinButton = new SpinButton
			{
				GridColumn = 1,
				GridRow = 1,
				Width = 100,
				Nullable = true
			};
			grid.Widgets.Add(spinButton);

			// Add it to the desktop
			this.desktop = new Desktop();
			this.desktop.Root = grid;

		}

		public void Draw(GameTime gameTime)
		{
			/**
			this.ui.BeginLayout(gameTime);
			this.drawMethod(this, gameTime);
			this.ui.EndLayout();
			*/
			this.desktop.Render();
		}
	}
}
