using FrozenEngine.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS.Components
{
	public abstract class Renderer : Component
	{
		[RequiredComponent]
		public Transform Transform { get; protected set; }

		public abstract Rectangle Bounds { get; }
		public abstract long RendererSortedHash { get; }
		public abstract void Draw(DrawingSystem drawing);
		public abstract void UpdateRenderer(GameTime gameTime);
		protected override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			this.UpdateRenderer(gameTime);
		}
	}
}
