using FrozenEngine.Drawing;
using FrozenEngine.ECS.Systems;
using FrozenEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.ECS.Components
{
	public class InfinitePlaneRenderer : Renderer
	{
		private VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[6];

		public Material Material { get; set; }
		public Color ColorTint { get; set; } = Color.White;

		public override long RendererSortedHash => this.Material.DefaultSortingHash(this.Transform.Position.Z);
		public override Rectangle Bounds => CoreMath.InfiniteRectangle;

		public override void Draw(DrawingSystem drawing)
		{
			// drawing.AddTexturedQuad(Texture)
		}

		public override void UpdateRenderer(GameTime gameTime)
		{
			Camera c = this.Entity.Scene.GetComponents<Camera>().FirstOrDefault();
			if (c != null)
			{
				Vector3 currentPosition = Vector3.Transform(this.Transform.Position, c.Projection);
			}
		}
	}
}
