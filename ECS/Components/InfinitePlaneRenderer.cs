using System.Collections.Generic;
using Frozen.Drawing;
using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.ECS.Components
{
	public class InfinitePlaneRenderer : Renderer
	{
		private readonly TriangleList _tList = new TriangleList();

		private readonly VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[4];

		public override Rectangle Bounds => FrozenMath.InfiniteRectangle;

		public Color ColorTint { get; set; } = Color.White;

		public Material Material { get; set; }

		public override long RendererSortedHash => Material.DefaultSortingHash(Transform.Position.Z);

		private IEnumerable<PrimitiveItem> DrawForCamera(Camera camera)
		{
			Vector2 bound = camera.Viewport.Bounds.Size.ToVector2();
			Vector3 topLeft = camera.ScreenToWorld(Vector2.Zero, Transform.WorldPosition.Z);
			Vector3 topRight = camera.ScreenToWorld(bound * Vector2.UnitX, Transform.WorldPosition.Z);
			Vector3 bottomLeft = camera.ScreenToWorld(bound * Vector2.UnitY, Transform.WorldPosition.Z);
			Vector3 bottomRight = camera.ScreenToWorld(bound, Transform.WorldPosition.Z);

			float minX = FrozenMath.Min(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
			float maxX = FrozenMath.Max(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
			float minY = FrozenMath.Min(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
			float maxY = FrozenMath.Max(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);

			Engine.Log.Core.Debug($"{camera.Entity.Name} {minX} {maxX} {minY} {maxY}");

			float width = maxX - minX;
			float height = maxY - minY;

			Vector2 txSize = Material.SpriteSheet.Texture.Bounds.Size.ToVector2() * Transform.Scale;
			Vector2 repetitions = new Vector2(width, height) / txSize;

			Vector2 tlTexture = (new Vector2(minX, minY) - Transform.Position.XY()) / txSize;

			_vertices[0].Position = new Vector3(minX, minY, Transform.WorldPosition.Z);
			_vertices[1].Position = new Vector3(maxX, minY, Transform.WorldPosition.Z);
			_vertices[2].Position = new Vector3(minX, maxY, Transform.WorldPosition.Z);
			_vertices[3].Position = new Vector3(maxX, maxY, Transform.WorldPosition.Z);
			_vertices[0].Color = ColorTint;
			_vertices[1].Color = ColorTint;
			_vertices[2].Color = ColorTint;
			_vertices[3].Color = ColorTint;
			_vertices[0].TextureCoordinate = tlTexture;
			_vertices[1].TextureCoordinate = tlTexture + Vector2.UnitX * repetitions;
			_vertices[2].TextureCoordinate = tlTexture + Vector2.UnitY * repetitions;
			_vertices[3].TextureCoordinate = tlTexture + repetitions;

			_tList.Clean();
			_tList.Reset(Material);
			_tList.AppendVertices(_vertices, Renderer.QUAD_INDICES);
			yield return _tList;
		}

		public override void Draw(DrawingSystem drawing)
		{
			drawing.DrawCameraBoundPrimitives(DrawForCamera);
		}

		public override void UpdateRenderer()
		{ }
	}
}
