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
	public abstract class UI : Component
	{
		protected readonly Desktop desktop;

		public UI()
		{
			this.desktop = this.BuildUI();
		}

		public void Draw(GameTime gameTime)
		{
			this.desktop.Render();
		}

		protected abstract Desktop BuildUI();

		protected override void OnUpdate(GameTime gameTime)
		{
			if(this.desktop.IsMouseOverGUI)
			{
				int a = 0;
			}	
		}
	}
}
