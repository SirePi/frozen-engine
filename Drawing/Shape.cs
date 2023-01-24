namespace Frozen.Drawing
{
	/*
	public abstract class Shape
	{
		protected VertexPositionColor[] vertices;
		protected int[] indices;

		public VertexPositionColor[] Vertices => vertices;
		public int[] Indices => indices;

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
			get => width;
			set
			{
				width = value;
				halfWidth = value / 2;
			}
		}
		public float Height
		{
			get => height;
			set
			{
				height = value;
				halfHeight = value / 2;
			}
		}

		public Rectangle()
		{
			vertices = new VertexPositionColor[4];
			indices = RectangleIndices;
		}

		public override void UpdateVertices(Color color, Matrix transform)
		{
			vertices[0].Color = color;
			vertices[0].Position = Vector3.Transform(new Vector3(-halfWidth, -halfHeight, 0), transform);
			vertices[1].Color = color;
			vertices[1].Position = Vector3.Transform(new Vector3(halfWidth, -halfHeight, 0), transform);
			vertices[2].Color = color;
			vertices[2].Position = Vector3.Transform(new Vector3(-halfWidth, halfHeight, 0), transform);
			vertices[3].Color = color;
			vertices[3].Position = Vector3.Transform(new Vector3(halfWidth, halfHeight, 0), transform);
		}
	}
	*/
}
