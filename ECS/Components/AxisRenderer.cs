using FrozenEngine.Drawing;
using FrozenEngine.ECS.Systems;
using FrozenEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS.Components
{
    public class AxisRenderer : Renderer
    {
		private static readonly int[] indices = new int[] { 0, 1, 0, 2, 0, 3 };

		private readonly VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
        public override Rectangle Bounds => CoreMath.InfiniteRectangle;

		public override long RendererSortedHash => (long)this.Transform.Position.Z << 32;

        public override void Draw(DrawingSystem drawing)
        {
			drawing.DrawLines(this.vertices, indices);
        }

        public override void UpdateRenderer(GameTime gameTime)
        {
			Matrix matrix = this.Transform.FullTransformMatrix;

			this.vertices[0].Color = Color.White;
			this.vertices[0].Position = Vector3.Transform(Vector3.Zero, matrix);
			this.vertices[1].Color = Color.Red;
			this.vertices[1].Position = Vector3.Transform(Vector3.UnitX * 500, matrix);
			this.vertices[2].Color = Color.Green;
			this.vertices[2].Position = Vector3.Transform(Vector3.UnitY * 500, matrix);
			this.vertices[3].Color = Color.Blue;
			this.vertices[3].Position = Vector3.Transform(Vector3.UnitZ * 500, matrix);
		}
    }
}
