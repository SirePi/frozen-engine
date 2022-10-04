using Frozen.Drawing;
using Frozen.ECS.Systems;
using Frozen.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.ECS.Components
{
	public class InfinitePlaneRenderer : Renderer
	{
		private readonly TriangleList tList = new TriangleList();
		private readonly VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];

		public Material Material { get; set; }
		public Color ColorTint { get; set; } = Color.White;

		public override long RendererSortedHash => this.Material.DefaultSortingHash(this.Transform.Position.Z);
		public override Rectangle Bounds => CoreMath.InfiniteRectangle;

		public override void Draw(DrawingSystem drawing)
		{
			drawing.DrawCameraBoundPrimitives(this.DrawForCamera);
		}

		private IEnumerable<PrimitiveItem> DrawForCamera(Camera camera)
		{
			Vector2 bound = camera.Viewport.Bounds.Size.ToVector2();
			Vector3 topLeft = camera.ScreenToWorld(Vector2.Zero, this.Transform.WorldPosition.Z);
			Vector3 topRight = camera.ScreenToWorld(bound * Vector2.UnitX, this.Transform.WorldPosition.Z);
			Vector3 bottomLeft = camera.ScreenToWorld(bound * Vector2.UnitY, this.Transform.WorldPosition.Z);
			Vector3 bottomRight = camera.ScreenToWorld(bound, this.Transform.WorldPosition.Z);

			float minX = CoreMath.Min(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
			float maxX = CoreMath.Max(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
			float minY = CoreMath.Min(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
			float maxY = CoreMath.Max(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);

			Engine.Log.Core.Debug($"{camera.Entity.Name} {minX} {maxX} {minY} {maxY}");

			float width = maxX - minX;
			float height = maxY - minY;

			Vector2 txSize = this.Material.SpriteSheet.Texture.Bounds.Size.ToVector2() * this.Transform.Scale;
			Vector2 repetitions = new Vector2(width, height) / txSize;  

			Vector2 tlTexture = (new Vector2(minX, minY) - this.Transform.Position.XY()) / txSize;

			this.vertices[0].Position = new Vector3(minX, minY, this.Transform.WorldPosition.Z);
			this.vertices[1].Position = new Vector3(maxX, minY, this.Transform.WorldPosition.Z);
			this.vertices[2].Position = new Vector3(minX, maxY, this.Transform.WorldPosition.Z);
			this.vertices[3].Position = new Vector3(maxX, maxY, this.Transform.WorldPosition.Z);
			this.vertices[0].Color = this.ColorTint;
			this.vertices[1].Color = this.ColorTint;
			this.vertices[2].Color = this.ColorTint;
			this.vertices[3].Color = this.ColorTint;
			this.vertices[0].TextureCoordinate = tlTexture;
			this.vertices[1].TextureCoordinate = tlTexture + Vector2.UnitX * repetitions;
			this.vertices[2].TextureCoordinate = tlTexture + Vector2.UnitY * repetitions;
			this.vertices[3].TextureCoordinate = tlTexture + repetitions;

			this.tList.Clean();
			this.tList.Reset(this.Material);
			this.tList.AppendVertices(this.vertices, Renderer.QUAD_INDICES);
			yield return this.tList;
		}

		public override void UpdateRenderer()
		{ }
	}
}
