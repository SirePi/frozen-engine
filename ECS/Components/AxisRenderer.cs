using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.ECS.Components
{
	public class AxisRenderer : Renderer
	{
		private static readonly int[] LINE_INDICES = new int[] { 0, 1, 0, 2, 0, 3 };

		private readonly VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[4];

		public override Rectangle Bounds => FrozenMath.InfiniteRectangle;

		public override long RendererSortedHash => (long)Transform.Position.Z << 32;

		public override void Draw(DrawingSystem drawing)
		{
			drawing.DrawLines(_vertices, AxisRenderer.LINE_INDICES);
		}

		public override void UpdateRenderer()
		{
			Matrix matrix = Transform.FullTransformMatrix;

			_vertices[0].Color = Color.White;
			_vertices[0].Position = Vector3.Transform(Vector3.Zero, matrix);
			_vertices[1].Color = Color.Red;
			_vertices[1].Position = Vector3.Transform(Vector3.UnitX * 500, matrix);
			_vertices[2].Color = Color.Green;
			_vertices[2].Position = Vector3.Transform(Vector3.UnitY * 500, matrix);
			_vertices[3].Color = Color.Blue;
			_vertices[3].Position = Vector3.Transform(Vector3.UnitZ * 500, matrix);
		}
	}
}
