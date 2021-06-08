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
			drawing.DrawCameraBoundPrimitives(this.DrawForCamera);
		}

		private IEnumerable<PrimitiveDrawable> DrawForCamera(Camera camera)
		{
			//Vector3 center = camera.ScreenToWorld(this.Transform.Position);
			//Vector3 topLeft = camera.ScreenToWorld(new Vector3(camera.RenderTarget.Bounds.Location.ToVector2(), 0));
			//Vector3 bottomRight = camera.ScreenToWorld(new Vector3(camera.RenderTarget.Bounds.Size.ToVector2(), 0));

			yield break;
		}

		public override void UpdateRenderer(GameTime gameTime)
		{ }
	}
}
