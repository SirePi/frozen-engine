using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;

namespace Frozen.Drawing
{
	/*
	public abstract class Shape
	{
		protected VertexPositionColor[] vertices;
		protected int[] indices;

		public VertexPositionColor[] Vertices => this.vertices;
		public int[] Indices => this.indices;

		public abstract void UpdateVertices(Color color, Matrix transform);
	}

	public sealed class Rectangle : Shape
	{
		private static readonly int[] RectangleIndices = new int[] { 0, 1, 2, 1, 3, 2 };
		private float width;
		private float height;
		private float halfWidth;
		private float halfHeight;
		public float Width
		{
			get => this.width;
			set
			{
				this.width = value;
				this.halfWidth = value / 2;
			}
		}
		public float Height
		{
			get => this.height;
			set
			{
				this.height = value;
				this.halfHeight = value / 2;
			}
		}

		public Rectangle()
		{
			this.vertices = new VertexPositionColor[4];
			this.indices = RectangleIndices;
		}

		public override void UpdateVertices(Color color, Matrix transform)
		{
			this.vertices[0].Color = color;
			this.vertices[0].Position = Vector3.Transform(new Vector3(-this.halfWidth, -this.halfHeight, 0), transform);
			this.vertices[1].Color = color;
			this.vertices[1].Position = Vector3.Transform(new Vector3(this.halfWidth, -this.halfHeight, 0), transform);
			this.vertices[2].Color = color;
			this.vertices[2].Position = Vector3.Transform(new Vector3(-this.halfWidth, this.halfHeight, 0), transform);
			this.vertices[3].Color = color;
			this.vertices[3].Position = Vector3.Transform(new Vector3(this.halfWidth, this.halfHeight, 0), transform);
		}
	}
	*/
}
