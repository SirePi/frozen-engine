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
    public class ShapeRenderer : Renderer
    {
		private readonly VertexPositionColor[] vertices;
		private int size;
		private Rectangle bounds;

		public Shape Shape { get; set; }
		public Color ColorTint { get; set; } = Color.White;
		public int Size 
		{
			get => this.size;
			set
			{
				this.size = value;
				this.bounds = new Rectangle(-value / 2, -value / 2, value, value);
			} 
		}
        public override Rectangle Bounds => this.bounds;

		public override long RendererSortedHash => (long)this.Transform.Position.Z << 32;

		public ShapeRenderer(Shape shape)
		{
			this.Shape = shape;
			switch(this.Shape)
			{
				case Shape.Circle: this.vertices = new VertexPositionColor[24]; break;
				case Shape.Cross: this.vertices = new VertexPositionColor[4]; break;
				case Shape.Square: this.vertices = new VertexPositionColor[8]; break;
			}
		}

        public override void Draw(DrawingSystem drawing)
        {
            drawing.AddColoredLines(this.vertices);
        }

        public override void UpdateRenderer(GameTime gameTime)
        {
			Matrix matrix = this.Transform.FullTransformMatrix;

			switch(this.Shape)
			{
				case Shape.Circle:
					for(int i = 0; i < 12; i++)
					{
						int idx = i * 2;
						this.vertices[idx].Color = this.ColorTint;
						this.vertices[idx].Position = Vector3.Transform(new Vector3(CoreMath.CirclePoints[i] * this.Size, 0), matrix);

						if (i == 0)
						{
							this.vertices[this.vertices.Length - 1].Color = this.ColorTint;
							this.vertices[this.vertices.Length - 1].Color = this.ColorTint;
						}

					}
					break;
			}
			this.vertices[0].Color = this.ColorTint;
			this.vertices[0].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Top, 0), matrix);
			this.vertices[1].Color = this.ColorTint;
			this.vertices[1].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Top, 0), matrix);
			this.vertices[2].Color = this.ColorTint;
			this.vertices[2].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Bottom, 0), matrix);

			this.vertices[3] = this.vertices[1];
			this.vertices[4].Color = this.ColorTint;
			this.vertices[4].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Bottom, 0), matrix);
			this.vertices[5] = this.vertices[2];
		}
    }
}
