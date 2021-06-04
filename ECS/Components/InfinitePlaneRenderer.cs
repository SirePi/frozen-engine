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
			Matrix inverseProjection = Matrix.Invert(camera.Projection);
			Matrix inverseView = Matrix.Invert(camera.View);

			Vector3 topLeft = Vector3.Transform(Vector3.Transform(new Vector3(camera.RenderTarget.Bounds.Left, camera.RenderTarget.Bounds.Top, this.Transform.WorldPosition.Z), inverseProjection), inverseView);

			Vector3 wut = Vector3.Transform(Vector3.Transform(topLeft, camera.View), camera.Projection);

			Vector3 bottomRight = Vector3.Transform(Vector3.Transform(new Vector3(camera.RenderTarget.Bounds.Right, camera.RenderTarget.Bounds.Bottom, this.Transform.WorldPosition.Z), inverseProjection), inverseView);

			yield break;
		}

		public override void UpdateRenderer(GameTime gameTime)
		{ }
	}
}
