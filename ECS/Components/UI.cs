using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS.Components
{
	public class UI : Component
	{
		private readonly ImGUIRenderer ui;

		private readonly Action<UI, GameTime> drawMethod;

		public UI(Action<UI, GameTime> drawMethod)
		{
			this.drawMethod = drawMethod;
			this.ui = new ImGUIRenderer(Core.Game).Initialize().RebuildFontAtlas();
		}

		public void Draw(GameTime gameTime)
		{
			this.ui.BeginLayout(gameTime);
			this.drawMethod(this, gameTime);
			this.ui.EndLayout();
		}
	}
}
