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
		private readonly TriangleList tList = new TriangleList();
		private readonly VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
		private readonly int[] indices = new int[] { 0, 1, 2, 1, 3, 2 };

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
			Vector3 topLeft = camera.ScreenToWorld(Vector2.Zero, this.Transform.WorldPosition.Z);
			Vector3 bottomRight = camera.ScreenToWorld(camera.Viewport.Bounds.Size.ToVector2(), this.Transform.WorldPosition.Z);

			Vector2 txSize = this.Material.SpriteSheet.Texture.Bounds.Size.ToVector2();
			Vector2 repetitions = (bottomRight - topLeft).XY() / txSize;  

			Vector2 tlTexture = (topLeft - this.Transform.Position).XY() / txSize;

			this.vertices[0].Position = topLeft;
			this.vertices[1].Position = new Vector3(bottomRight.X, topLeft.Y, topLeft.Z);
			this.vertices[2].Position = new Vector3(topLeft.X, bottomRight.Y, topLeft.Z);
			this.vertices[3].Position = bottomRight;
			this.vertices[0].Color = Color.Yellow;
			this.vertices[1].Color = Color.Red;
			this.vertices[2].Color = Color.Blue;
			this.vertices[3].Color = Color.Green;
			this.vertices[0].TextureCoordinate = tlTexture;
			this.vertices[1].TextureCoordinate = tlTexture + Vector2.UnitX * repetitions;
			this.vertices[2].TextureCoordinate = tlTexture + Vector2.UnitY * repetitions;
			this.vertices[3].TextureCoordinate = tlTexture + repetitions;

			this.tList.Clean();
			this.tList.Reset(this.Material);
			this.tList.AppendVertices(this.vertices, this.indices);
			yield return this.tList;
		}

		public override void UpdateRenderer(GameTime gameTime)
		{ }
	}
}
